namespace Sandbox;

public class DraggingHandState : HandState
{
	[Property] public float MinDragDistance { get; set; } = 0.2f;
	[Property] public float MaxDragDistance { get; set; } = 1f;
	[Property] public float DragForce { get; set; } = 50f;
	[Property] public float AngularVelocityDamping { get; set; } = 2.5f;
	[Property] public bool DebugDraw { get; set; }
	/// <summary>
	/// A factor applied to the velocity of the dragged object when it is 
	/// no longer being dragged.
	/// </summary>
	[Property, Range(0f, 5f, 0.05f)] 
	public float ThrowSpeedFactor { get; set; } = 1.5f;
	/// <summary>
	/// Determines how much the dragged object will spin when it is released.
	/// This spin is scaled by the velocity of the object.
	/// </summary>
	[Property, Range(0f, 0.25f, 0.01f)] 
	public float ThrowSpinFactor { get; set; } = 0.03f;

	public GameObject Dragged { get; set; }

	public GameObject DragSource { get; set; }
	public Ray DragRay => new( 
				origin: DragSource.Transform.Position, 
				direction: DragSource.Transform.Rotation.Forward 
			);
	
	public float CurrentDragDistance 
	{
		get => _currentDragDistance;
		set
		{
			_currentDragDistance = value.Clamp( InteractionReach * MinDragDistance, InteractionReach * MaxDragDistance );
		}
	}
	private float _currentDragDistance;
	private PhysicsComponent DraggedRigidbody { get; set; }
	private GameObject _originalParent;

	public void Initialize( GameObject dragged, GameObject dragSource)
	{
		_originalParent = GameObject.Parent;
		GameObject.Parent = dragged;
		// In case the dragged has no hold data, reset the transform.
		Transform.Position = dragged.Transform.Position;
		Transform.Rotation = Rotation.Identity;
		// The parent should never affect the scale of the hand.
		Transform.Scale = 1f;

		if ( HandModel is not null )
			HandModel.Enabled = true;

		Dragged = dragged;
		DragSource = dragSource;
		DraggedRigidbody = Dragged.GetComponent<PhysicsComponent>();
		CurrentDragDistance = dragged.Transform.Position.Distance( dragSource.Transform.Position );

		InputGlyphsPanel.Instance.AddGlyph( new InputGlyphData()
		{
			ActionName = "attack2",
			DisplayText = "Rotate",
			RemovalPredicate = () => this?.Enabled != true
		} );

		Controller.BlockThirdPerson( this );
	}

	public override void OnDisabled()
	{
		Dragged?.GetComponent<LookRotateComponent>()?.SetEnabled( false );
		if ( DraggedRigidbody is not null )
		{
			DraggedRigidbody.Velocity *= ThrowSpeedFactor;
			var spinStrength = DraggedRigidbody.Velocity.Length * ThrowSpinFactor;
			DraggedRigidbody.AngularVelocity += Vector3.Random * spinStrength;
		}

		GameObject.Parent = _originalParent;
		Dragged = null;
		DragSource = null;
		DraggedRigidbody = null;
		Controller.UnblockThirdPerson( this );
		Controller.UnblockLook( this );
	}

	public void DoDebugDraw()
	{
		if ( Dragged is null )
			return;

		Gizmo.Draw.Color = Color.White.WithAlpha( 0.4f );
		var toPosition = DragRay.Project( CurrentDragDistance );
		Gizmo.Draw.Line( Dragged.Transform.Position, toPosition );
		Gizmo.Draw.SolidSphere( toPosition, 1f );
	}

	public override void Update()
	{
		if ( Dragged is null )
			return;

		var holdData = Dragged.GetComponent<HoldDataComponent>();
		var handAnimator = GetComponent<WorldHandAnimator>();
		if ( holdData is not null && handAnimator is not null )
		{
			// Set the hand position and rotation based on the hold data.
			holdData.UpdateHand( handAnimator );
		}
		DoDebugDraw();
	}

	public override void FixedUpdate()
	{
		DraggedRigidbody ??= Dragged.GetComponent<PhysicsComponent>();

		if ( Dragged is null || DraggedRigidbody is null )
		{
			ChangeState<EmptyHandState>();
			return;
		}

		UpdateDrag();

		if ( Input.Down( "attack2" ) )
		{
			DraggedRigidbody.AngularVelocity = Vector3.Zero;
			Dragged.GetOrAddComponent<LookRotateComponent>().SetEnabled( true );
			Controller.BlockLook( this );
		}
		else
		{
			Dragged.GetComponent<LookRotateComponent>()?.SetEnabled( false );
			Controller.UnblockLook( this );
		}

		UpdateDistance();
	}

	private void UpdateDrag()
	{
		var dragTo = DragRay.Project( CurrentDragDistance );
		var direction = (dragTo - DraggedRigidbody.Transform.Position).Normal;
		var distance = dragTo.Distance( DraggedRigidbody.Transform.Position );
		var scaledSpeed = MathX.Lerp( 10f, DragForce * 5, distance / 60f );
		DraggedRigidbody.Velocity = direction * scaledSpeed;
		// Gradually stop the rotation.
		DraggedRigidbody.AngularVelocity = DraggedRigidbody.AngularVelocity.LerpTo( Vector3.Zero, AngularVelocityDamping * Time.Delta );
	}

	private void UpdateDistance()
	{
		var mouseInput = Input.MouseWheel * 30.0f;
		if ( mouseInput != 0 )
		{
			CurrentDragDistance += mouseInput;
			var minDistance = MinDragDistance * InteractionReach;
			var maxDistance = MaxDragDistance * InteractionReach;
			CurrentDragDistance = CurrentDragDistance.Clamp( minDistance, maxDistance );
		}
	}

}
