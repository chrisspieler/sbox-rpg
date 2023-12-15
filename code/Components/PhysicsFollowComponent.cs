namespace Sandbox;

public class PhysicsFollowComponent : Component
{
	[Property] public Rigidbody Rigidbody { get; set; }
	[Property] public GameObject Target { get; set; }
	[Property] public float FollowForce { get; set; } = 50f;
	[Property] public float AngularVelocityDamping { get; set; } = 2.5f;
	[Property] public bool DebugDraw { get; set; }

	protected override void OnStart()
	{
		Rigidbody ??= Components.Get<Rigidbody>();
	}

	protected override void OnUpdate()
	{
		if ( DebugDraw )
			DoDebugDraw();
	}

	protected override void OnFixedUpdate()
	{
		if ( Target?.IsValid != true || Rigidbody?.Enabled != true )
			return;

		var goalPos = Target.Transform.Position;
		var direction = (goalPos - Transform.Position).Normal;
		var distance = goalPos.Distance( Transform.Position );
		var scaledSpeed = MathX.Lerp( 10f, FollowForce * 5, distance / 60f );
		Rigidbody.Velocity = direction * scaledSpeed;
		// Gradually stop the rotation.
		Rigidbody.AngularVelocity = Rigidbody.AngularVelocity.LerpTo( Vector3.Zero, AngularVelocityDamping * Time.Delta );
	}

	public void DoDebugDraw()
	{
		if ( Target?.IsValid != true )
			return;

		Gizmo.Draw.Color = Color.White.WithAlpha( 0.4f );
		var toPosition = Target.Transform.Position;
		Gizmo.Draw.Line( Transform.Position, toPosition );
		Gizmo.Draw.SolidSphere( toPosition, 1f );
	}
}
