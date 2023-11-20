namespace Sandbox;

public class RunState : GroundedState
{
	protected override void HandleUpdate()
	{
		if ( Controller.RunToggle )
		{
			if ( Input.Down( "run" ) )
			{
				ChangeState<WalkState>();
				return;
			}
		}
		else if ( !Input.Down( "run" ) )
		{
			ChangeState<WalkState>();
			return;
		}
	}

	protected override void SetAnimation( CitizenAnimation animation )
	{
		var cc = Controller.CharacterController;
		// Scale down velocity to avoid leaning forward too much.
		animation.WithVelocity( cc.Velocity * 0.7f );
		animation.IsGrounded = true;
		animation.WithLook( Controller.EyeAngles.Forward, 1, 0.5f, 0.5f );
		animation.MoveStyle = CitizenAnimation.MoveStyles.Run;
	}
}
