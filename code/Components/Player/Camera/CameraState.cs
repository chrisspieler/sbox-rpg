namespace Sandbox;

public class CameraState : PlayerState
{
	[Property, Range( 20f, 150f )] public float FieldOfView { get; set; } = 80f;
	protected CameraComponent Camera => Player.Components.Get<CameraComponent>( FindMode.EverythingInSelfAndDescendants );
}
