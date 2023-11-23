﻿namespace Sandbox;

public class HoldDataComponent : BaseComponent
{
	[Property] public Vector3 HoldPosition { get; set; }
	[Property] public Rotation HoldRotation { get; set; }
	[Property, Range( 0, 1 )] public float FingerCurl { get; set; } = 0.6f;
	[Property] public bool DebugDraw { get; set; }

	public override void DrawGizmos()
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

		var otherHoldData = hand.GetComponent<HoldDataComponent>();
		if ( otherHoldData is null )
		{
			hand.Transform.Position = Transform.Position + HoldPosition;
			hand.Transform.Rotation = Transform.Rotation * HoldRotation;
			return;
		}
		var thisHoldPos = Transform.World.PointToWorld( HoldPosition );
		hand.Transform.Position = thisHoldPos;
		hand.Transform.Rotation = Transform.Rotation * HoldRotation * otherHoldData.HoldRotation.Inverse;
		hand.WithAllFingerCurl( FingerCurl );

		if ( !DebugDraw && !otherHoldData.DebugDraw )
			return;

		Gizmo.Draw.Color = Color.Cyan;
		Gizmo.Draw.SolidSphere( thisHoldPos, 0.5f );
	}
}