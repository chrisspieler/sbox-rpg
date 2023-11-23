namespace Sandbox;

public class DraggingHandState : HandState
{
	[Property] public float MinDragDistance { get; set; } = 0.2f;
	[Property] public float MaxDragDistance { get; set; } = 1f;
	[Property] public float DragForce { get; set; } = 50f;
	[Property] public float AngularVelocityDamping { get; set; } = 2.5f;
	[Property] public bool DebugDraw { get; set; }

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
		// TODO: Get position, rotation and finger curl from dragged.
		Transform.Position = dragged.Transform.Position;
		Transform.Rotation = Rotation.Identity;
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
			RemovalPredicate = () => this?.Enabled == true
		} );

		Controller.BlockThirdPerson( this );
	}

	public override void OnDisabled()
	{
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
			UpdateRotate();
			Controller.BlockLook( this );
		}
		else
		{
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

	private void UpdateRotate()
	{
		DraggedRigidbody.AngularVelocity = Vector3.Zero;

		// Yoinked rotation code from the Sandbox physgun.
		var eyeRot = Rotation.From( new Angles( 0f, Camera.Main.Rotation.Yaw(), 0f ) );
		var localRot = eyeRot;
		localRot *= Rotation.FromAxis( Vector3.Up, Input.MouseDelta.x * 0.3f );
		localRot *= Rotation.FromAxis( Vector3.Right, Input.MouseDelta.y * 0.3f );
		localRot = eyeRot.Inverse * localRot;
		Dragged.Transform.Rotation = localRot * Dragged.Transform.Rotation;
		// Clear the mouse input.
		Input.MouseDelta = Vector2.Zero;
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
