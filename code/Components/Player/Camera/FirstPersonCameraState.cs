namespace Sandbox;

public class FirstPersonCameraState : CameraState
{
	protected override void OnUpdate()
	{
		if ( Input.Pressed( "flashlight" ) && !Controller.IsThirdPersonBlocked )
		{
			Input.Clear( "flashlight" );
			StateMachine.ChangeState<ThirdPersonCameraState>();
			return;
		}

		Controller.PlayerCam.RenderExcludeTags.Add( "thirdperson" );
		Controller.PlayerCam.FieldOfView = FieldOfView;

		var mouseInput = Input.MouseWheel * 30.0f;
		if ( mouseInput.y < 0 && !Controller.IsThirdPersonBlocked )
		{
			var thirdPerson = StateMachine.ChangeState<ThirdPersonCameraState>();
			thirdPerson.Distance = 50f;
			return;
		}
		Controller.PlayerCam.Transform.Position = Controller.Eye.Transform.Position;
		Controller.PlayerCam.Transform.Rotation = Controller.EyeAngles.ToRotation();
	}
}
