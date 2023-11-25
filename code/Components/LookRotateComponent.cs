namespace Sandbox;

/// <summary>
/// A component that rotates its GameObject using AnalogLook.
/// </summary>
public class LookRotateComponent : BaseComponent
{
	[Property, Range(0, 1080f, 20f)] public float XSpeed { get; set; } = 720f;
	[Property] public bool InvertX { get; set; } = true;
	[Property, Range(0, 1080f, 20f)] public float YSpeed { get; set; } = 720f;
	[Property] public bool InvertY { get; set; } = false;

	public override void FixedUpdate()
	{
		var inputVec = new Vector2( Input.AnalogLook.yaw, Input.AnalogLook.pitch );
		var xInput = inputVec.x * XSpeed * Time.Delta * (InvertX ? -1 : 1);
		var yInput = inputVec.y * YSpeed * Time.Delta * (InvertY ? -1 : 1);
		var worldTx = Transform.World;
		var upRotation = Rotation.FromAxis( Camera.Main.Rotation.Up, xInput );
		var rightRotation = Rotation.FromAxis( Camera.Main.Rotation.Right, yInput );
		Transform.Rotation = worldTx
			.RotateAround( worldTx.Position, upRotation )
			.RotateAround( worldTx.Position, rightRotation )
			.Rotation;
	}
}
