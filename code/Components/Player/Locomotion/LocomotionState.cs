using Sandbox.Citizen;

namespace Sandbox;

public abstract class LocomotionState : PlayerState
{
	[Property, Range(0, 800)] public float MoveSpeed { get; set; } = 160f;
	[Property] public bool IgnoreZ { get; set; } = true;
	[Property, Range(0.1f, 6.0f, 0.1f)] public float Friction { get; set; } = 2.0f;
	[Property] public bool RotateBodyWithCamera { get; set; } = true;
	public Vector3 WishVelocity { get; protected set; }

	protected virtual Vector3 GetMovementDirection( )
	{
		if ( Controller.IsMovementBlocked )
			return Vector3.Zero;

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

	protected virtual void RotateBody()
	{
		var body = Controller.Body;
		if ( body is null )
			return;

		var targetAngle = new Angles( 0, Controller.EyeAngles.yaw, 0 ).ToRotation();
		if ( Controller.IsFirstPerson )
		{
			body.Transform.Rotation = targetAngle;
			return;
		}

		var cc = Controller.CharacterController;
		var v = cc.Velocity.WithZ( 0 );

		if ( v.Length > 10.0f )
		{
			targetAngle = Rotation.LookAt( v, Vector3.Up );
		}

		var rotateDifference = body.Transform.Rotation.Distance( targetAngle );

		var isMovingQuickly = cc.Velocity.Length > 10.0f;
		if ( rotateDifference > 50.0f || isMovingQuickly )
		{
			var rotationSpeedFactor = 2.0f + cc.Velocity.Length / 5.0f;
			body.Transform.Rotation = Rotation.Lerp( body.Transform.Rotation, targetAngle, Time.Delta * rotationSpeedFactor );
		}
	}

	protected override void OnUpdate()
	{
		if ( RotateBodyWithCamera )
		{
			RotateBody();
		}
		HandleUpdate();
		var animation = Controller.AnimationHelper;
		if ( animation is not null )
			SetAnimation( animation );
	}

	protected override void OnFixedUpdate()
	{
		var moveDir = GetMovementDirection();
		WishVelocity = moveDir * MoveSpeed;

		FinalizeMovement();
	}

	protected abstract void HandleUpdate();
	protected abstract void SetAnimation( CitizenAnimationHelper animation );
	/// <summary>
	/// Set the properties of the character controller or perform
	/// a state change if necessary.
	/// </summary>
	protected abstract void FinalizeMovement();
}
