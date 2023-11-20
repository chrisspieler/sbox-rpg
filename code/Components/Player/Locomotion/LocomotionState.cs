namespace Sandbox;

public abstract class LocomotionState : PlayerState
{
	[Property, Range(0, 800)] public float MoveSpeed { get; set; } = 160f;
	[Property] public bool IgnoreZ { get; set; } = true;
	[Property, Range(0.1f, 6.0f, 0.1f)] public float Friction { get; set; } = 2.0f;
	public Vector3 WishVelocity { get; protected set; }

	protected virtual Vector3 GetMovementDirection( )
	{
		var eyeRotation = Controller.EyeAngles.ToRotation();
		var inputVec = Input.AnalogMove;
		if ( Controller.EnableAutoWalk )
			inputVec.x = 1f;
		// Rotate the input vector so that forward is the direction the player is looking.
		var moveDir = inputVec * eyeRotation;
		// Remove Z component if we're not doing noclip/levitate style movement.
		if ( IgnoreZ ) moveDir = moveDir.WithZ( 0f );
		// Normalize the direction vector so that diagonal movement isn't faster.
		if ( !moveDir.IsNearlyZero() ) moveDir = moveDir.Normal;
		return moveDir;
	}

	public override void Update()
	{
		HandleUpdate();
		var animation = Controller.AnimationHelper;
		if ( animation is not null )
			SetAnimation( animation );
	}

	public override void FixedUpdate()
	{
		var moveDir = GetMovementDirection();
		WishVelocity = moveDir * MoveSpeed;

		FinalizeMovement();
	}

	protected abstract void HandleUpdate();
	protected abstract void SetAnimation( CitizenAnimation animation );
	/// <summary>
	/// Set the properties of the character controller or perform
	/// a state change if necessary.
	/// </summary>
	protected abstract void FinalizeMovement();
}
