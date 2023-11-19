namespace Sandbox;

public class FirstPersonCameraState : CameraState
{
	public override void Update()
	{
		if ( Input.Pressed( "flashlight" ) )
		{
			Input.Clear( "flashlight" );
			StateMachine.ChangeState<ThirdPersonCameraState>();
			return;
		}

		Controller.SetBodyTransparency( 0f );
		Controller.PlayerCam.FieldOfView = FieldOfView;

		var mouseInput = Input.MouseWheel * 30.0f;
		if ( mouseInput < 0 )
		{
			var thirdPerson = StateMachine.ChangeState<ThirdPersonCameraState>();
			thirdPerson.Distance = 50f;
			return;
		}
		Controller.PlayerCam.Transform.Position = Controller.Eye.Transform.Position;
		Controller.PlayerCam.Transform.Rotation = Controller.EyeAngles.ToRotation();
	}
}
