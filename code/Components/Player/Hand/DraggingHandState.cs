namespace Sandbox;

public class DraggingHandState : HandState
{
	[Property] public float MinDragDistance { get; set; } = 0.2f;
	[Property] public float MaxDragDistance { get; set; } = 1f;
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

	public GameObject DragTarget { get; set; }
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
	private Rigidbody DraggedRigidbody { get; set; }
	private GameObject _originalParent;

	public void Initialize( GameObject dragged, GameObject dragSource )
	{
		Dragged = dragged;
		DragSource = dragSource;
		DraggedRigidbody = Dragged.Components.Get<Rigidbody>();
		// Prevent the dragged object from showing the drag prompt.
		Dragged.Components.Get<DraggableComponent>().SetEnabled( false );
		// Prevent the movement code from treating held objects as obstacles.
		Dragged.Tags.Add( "held" );

		// The hand becomes the child of the held object, so store the original
		// parent for when we go back to the emptyhanded state.
		_originalParent = GameObject.Parent;
		GameObject.Parent = Dragged;

		ResetTransform();

		// The hand may be invisble in other states.
		if ( HandModel is not null )
			HandModel.Enabled = true;

		EnsureDragTargetExists();

		// The dragged object will be moved from the center by directly setting its velocity.
		var centerDrag = Dragged.Components.GetOrCreate<PhysicsFollowComponent>();
		centerDrag.Target = DragTarget;
		centerDrag.Rigidbody = DraggedRigidbody;

		CurrentDragDistance = Dragged.Transform.Position.Distance( DragSource.Transform.Position );

		InputGlyphsPanel.Instance.AddGlyph( new InputGlyphData()
		{
			ActionName = "attack2",
			DisplayText = "Rotate",
			RemovalPredicate = () => this?.Enabled != true
		} );

		Controller.BlockThirdPerson( this );
	}

	private void ResetTransform()
	{
		if ( Dragged.Components.Get<HoldDataComponent>() is null )
		{
			// Normally, the position and rotation of the hand is set by the
			// hold data of the held object. In case that data doesn't exist,
			// we provide a sensible default here.
			Transform.Position = Dragged.Transform.Position;
			Transform.Rotation = Rotation.Identity;
		}
		// The held object should never affect the scale of the hand.
		Transform.Scale = 1f;
	}

	private void EnsureDragTargetExists()
	{
		if ( DragTarget?.IsValid != true )
		{
			DragTarget = new GameObject( true, $"({GameObject.Name}) Drag Target" );
			DragTarget.Parent = GameObject;
		}
	}

	protected override void OnDisabled()
	{
		Dragged?.Tags?.Remove( "held" );

		Dragged?.Components.Get<LookRotateComponent>()?.SetEnabled( false );
		Dragged?.Components.Get<DraggableComponent>( true )?.SetEnabled( true );
		Dragged?.Components.Get<PhysicsFollowComponent>()?.Destroy();

		if ( DraggedRigidbody?.Enabled == true )
		{
			DraggedRigidbody.Velocity *= ThrowSpeedFactor;
			var spinStrength = DraggedRigidbody.Velocity.Length * ThrowSpinFactor;
			DraggedRigidbody.AngularVelocity += Vector3.Random * spinStrength;
		}

		GameObject.Parent = _originalParent;
		Dragged = null;
		DragSource = null;
		DraggedRigidbody = null;
		DragTarget?.Destroy();
	}

	protected override void OnUpdate()
	{
		if ( Dragged?.IsValid != true || !Dragged.Tags.Has( "held" ) || !Input.Down( "attack1" ) )
		{
			ChangeState<EmptyHandState>();
			return;
		}

		UpdateDistance();
		DragTarget.Transform.Position = DragRay.Project( CurrentDragDistance );

		var holdData = Dragged.Components.Get<HoldDataComponent>();
		var handAnimator = Components.Get<WorldHandAnimator>();
		if ( holdData is not null && handAnimator is not null )
		{
			// Set the hand position and rotation based on the hold data.
			holdData.UpdateHand( handAnimator );
		}
	}

	protected override void OnFixedUpdate()
	{
		DraggedRigidbody ??= Dragged.Components.Get<Rigidbody>();

		if ( Dragged?.IsValid != true || DraggedRigidbody is null )
		{
			ChangeState<EmptyHandState>();
			return;
		}

		if ( Input.Down( "attack2" ) )
		{
			DraggedRigidbody.AngularVelocity = Vector3.Zero;
			Dragged.Components.GetOrCreate<LookRotateComponent>().SetEnabled( true );
			Controller.BlockLook( this );
		}
		else
		{
			Dragged.Components.Get<LookRotateComponent>()?.SetEnabled( false );
			Controller.UnblockLook( this );
		}
	}

	private void UpdateDistance()
	{
		var mouseInput = Input.MouseWheel * 30.0f;
		if ( mouseInput != 0 )
		{
			CurrentDragDistance += mouseInput.y;
			var minDistance = MinDragDistance * InteractionReach;
			var maxDistance = MaxDragDistance * InteractionReach;
			CurrentDragDistance = CurrentDragDistance.Clamp( minDistance, maxDistance );
		}
	}
}
