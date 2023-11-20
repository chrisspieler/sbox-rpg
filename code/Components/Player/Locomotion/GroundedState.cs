namespace Sandbox;

public abstract class GroundedState : LocomotionState
{
	[Property] public bool CanJump { get; set; } = true;

	protected virtual void DoJump()
	{
		float flGroundFactor = 1.0f;
		float flMul = 268.3281572999747f * 1.2f;

		Controller.CharacterController.Punch( Vector3.Up * flMul * flGroundFactor );

		Controller.AnimationHelper?.TriggerJump();
		// TODO: Enter jumpsquat state?
		ChangeState<AirborneState>();
	}

	protected override void FinalizeMovement()
	{
		// TODO: Fix jump continuously refiring against low ceilings.
		if ( CanJump && Input.Down( "jump" ) )
		{
			DoJump();
			return;
		}

		var cc = Controller.CharacterController;

		if ( !cc.IsOnGround )
		{
			ChangeState<AirborneState>();
			return;
		}

		cc.Velocity = cc.Velocity.WithZ( 0 );
		cc.Accelerate( WishVelocity );
		cc.ApplyFriction( Friction );
		cc.Move();
		cc.Velocity = cc.Velocity.WithZ( 0 );
	}
}
