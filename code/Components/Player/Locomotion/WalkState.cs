using Sandbox.Citizen;

namespace Sandbox;

public class WalkState : GroundedState
{
	protected override void HandleUpdate()
	{
		if ( Controller.RunToggle )
		{
			if ( !Input.Down( "run" ) )
			{
				ChangeState<RunState>();
				return;
			}
		}
		else if ( Input.Pressed( "run" ) )
		{
			ChangeState<RunState>();
			return;
		}
	}

	protected override void SetAnimation( CitizenAnimationHelper animation )
	{
		var cc = Controller.CharacterController;
		animation.WithVelocity( cc.Velocity );
		animation.IsGrounded = true;
		animation.WithLook( Controller.EyeAngles.Forward, 1, 0.5f, 0.5f );
		animation.MoveStyle = CitizenAnimationHelper.MoveStyles.Walk;
	}
}
