namespace Sandbox;

public class HoldDataComponent : Component
{
	[Property] public Vector3 HoldPosition { get; set; }
	[Property] public Rotation HoldRotation { get; set; }
	[Property, Range( 0, 1 )] public float FingerCurl { get; set; } = 0.6f;
	[Property] public bool DebugDraw { get; set; }

	protected override void DrawGizmos()
	{
		Gizmo.Draw.Color = Color.Cyan;
		Gizmo.Draw.LineCylinder( HoldPosition, HoldPosition + HoldRotation.Forward * 0.25f, 2f, 2f, 16 );
		var line = new Line( HoldPosition, Vector3.Zero.WithX(1f) * HoldRotation, 5f );
		Gizmo.Draw.Line( line );
	}

	public void UpdateHand( WorldHandAnimator hand )
	{
		if ( hand.GameObject.Parent != GameObject )
			hand.GameObject.Parent = GameObject;

		var otherHoldData = hand.Components.Get<HoldDataComponent>();
		if ( otherHoldData is null )
		{
			hand.Transform.Position = hand.Transform.Position.LerpTo( Transform.Position + HoldPosition, Time.Delta * 10f );
			hand.Transform.Rotation = Transform.Rotation * HoldRotation;
			return;
		}
		var thisHoldPos = Transform.World.PointToWorld( HoldPosition );
		hand.Transform.Position = hand.Transform.Position.LerpTo( thisHoldPos, Time.Delta * 10f );
		hand.Transform.Rotation = Transform.Rotation * HoldRotation * otherHoldData.HoldRotation.Inverse;
		hand.WithAllFingerCurl( FingerCurl );

		if ( !DebugDraw && !otherHoldData.DebugDraw )
			return;

		Gizmo.Draw.Color = Color.Cyan;
		Gizmo.Draw.SolidSphere( thisHoldPos, 0.5f );
	}
}
