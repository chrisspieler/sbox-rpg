namespace Sandbox;

public class ThirdPersonCameraState : CameraState
{
	[Property, Range( 50, 400 )] public float Distance { get; set; } = 200f;
	[Property] public Vector3 Offset { get; set; } = Vector3.Zero.WithZ( -20f );

	protected override void OnUpdate()
	{
		if ( Input.Pressed( "flashlight" ) || Controller.IsThirdPersonBlocked )
		{
			Input.Clear( "flashlight" );
			StateMachine.ChangeState<FirstPersonCameraState>();
			return;
		}

		Controller.SetBodyTransparency( 1f );
		Controller.PlayerCam.FieldOfView = FieldOfView;

		var mouseInput = Input.MouseWheel * 30.0f;
		Distance -= mouseInput;
		Distance = Distance.Clamp( 49f, 400f );
		if ( Distance < 50f )
		{
			StateMachine.ChangeState<FirstPersonCameraState>();
			Distance = 50f;
			return;
		}
		var camPos = Controller.EyeRay.Project( -Distance );
		camPos += Offset;
		var tr = Scene.PhysicsWorld
			.Trace
			.Ray( Controller.EyeRay, -Distance )
			.Run();
		if ( tr.Hit )
		{
			camPos = tr.HitPosition;
		}
		Controller.PlayerCam.Transform.Position = camPos;
		Controller.PlayerCam.Transform.Rotation = Controller.EyeAngles.ToRotation();
	}
}
