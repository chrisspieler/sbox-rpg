namespace Sandbox;

/// <summary>
/// A component that rotates its GameObject using AnalogLook.
/// </summary>
public class LookRotateComponent : Component
{
	[Property, Range(0, 1080f, 20f)] public float XSpeed { get; set; } = 720f;
	[Property] public bool InvertX { get; set; } = true;
	[Property, Range(0, 1080f, 20f)] public float YSpeed { get; set; } = 720f;
	[Property] public bool InvertY { get; set; } = false;

	protected override void OnFixedUpdate()
	{
		var inputVec = new Vector2( Input.AnalogLook.yaw, Input.AnalogLook.pitch );
		var xInput = inputVec.x * XSpeed * Time.Delta * (InvertX ? -1 : 1);
		var yInput = inputVec.y * YSpeed * Time.Delta * (InvertY ? -1 : 1);
		var upRotation = Rotation.FromAxis( Camera.Main.Rotation.Up, xInput );
		var rightRotation = Rotation.FromAxis( Camera.Main.Rotation.Right, yInput );
		Transform.Rotation = Transform.World
			.RotateAround( Transform.Position, upRotation )
			.RotateAround( Transform.Position, rightRotation )
			.Rotation;
	}
}
