namespace Sandbox;

public class AirborneState : LocomotionState
{
	[Property] public Vector3 Gravity { get; set; } = Vector3.Zero.WithZ( 800f );
	[Property, Range( 0, 400 )] public float AirAccelerationLimit { get; set; } = 50f;

	protected override void HandleUpdate()
	{

	}

	protected override void SetAnimation( CitizenAnimation animation )
	{
		var cc = Controller.CharacterController;
		animation.WithVelocity( cc.Velocity );
		animation.IsGrounded = false;
		animation.WithLook( Controller.EyeAngles.Forward, 1, 0.5f, 0.5f );
	}

	protected override void FinalizeMovement()
	{
		var cc = Controller.CharacterController;

		if ( cc.IsOnGround )
		{
			// TODO: Enter a landing state instead?
			ChangeState<WalkState>();
			return;
		}

		cc.Velocity -= Gravity * Time.Delta * 0.5f;
		cc.Accelerate( WishVelocity.ClampLength( AirAccelerationLimit ) );
		cc.ApplyFriction( Friction );
		cc.Move();
		// Does this need to be done twice?
		cc.Velocity -= Gravity * Time.Delta * 0.5f;
	}
}
