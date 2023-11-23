namespace Sandbox;

/// <summary>
/// A component that rotates its GameObject using AnalogLook.
/// </summary>
public class LookRotateComponent : BaseComponent
{
	[Property, Range(0, 1080f, 20f)] public float XSpeed { get; set; } = 720f;
	[Property] public bool InvertX { get; set; } = false;
	[Property, Range(0, 1080f, 20f)] public float YSpeed { get; set; } = 720f;
	[Property] public bool InvertY { get; set; } = false;

	public override void FixedUpdate()
	{
		var inputVec = new Vector2( Input.AnalogLook.yaw, Input.AnalogLook.pitch );
		// This section of code is adapted from the Sandbox physgun rotation code.
		var eyeRot = Rotation.From( new Angles( 0f, Camera.Main.Rotation.Yaw(), 0f ) );
		var localRot = eyeRot;
		var xInput = inputVec.x * XSpeed * Time.Delta * (InvertX ? -1 : 1);
		localRot *= Rotation.FromAxis( Vector3.Up, xInput );
		var yInput = inputVec.y * YSpeed * Time.Delta * (InvertY ? -1 : 1);
		localRot *= Rotation.FromAxis( Vector3.Right, yInput );
		localRot = eyeRot.Inverse * localRot;
		Transform.Rotation = localRot * Transform.Rotation;

	}
}
