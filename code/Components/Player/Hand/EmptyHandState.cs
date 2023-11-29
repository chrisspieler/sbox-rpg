namespace Sandbox;

public class EmptyHandState : HandState
{
	[Property] public Vector3 CameraOffset { get; set; } = new Vector3( 20, 0f, 20f );
	[Property] public Rotation DefaultRotation { get; set; } = Rotation.From( 0f, 0f, 0f );
	[Property] public bool DebugDraw { get; set; }

	public GameObject Hovered { get; private set; }

	protected override void OnUpdate()
	{
		HandleAnimation();
		HandleInteraction();
	}

	private void HandleAnimation()
	{
		HandModel?.SetEnabled( Controller.IsFirstPerson );
		var targetPosition = Controller.Eye.Transform.Position;
		var offset = CameraOffset.WithZ( CameraOffset.z + MathF.Sin( Time.Now * 2f ) );
		targetPosition += offset * Controller.Eye.Transform.Rotation;
		Transform.Position = Transform.Position.LerpTo( targetPosition, Time.Delta * 10f );
		Transform.Rotation = DefaultRotation.Angles().WithYaw( Controller.Eye.Transform.Rotation.Yaw() ).ToRotation();
		Components.Get<WorldHandAnimator>()?.WithAllFingerCurl( 0.2f );
	}

	private void HandleInteraction()
	{
		if ( DialoguePanel.Instance.IsDialogueActive )
		{
			Unhover( Hovered );
			Hovered = null;
			return;
		}

		var tr = Scene.PhysicsWorld.Trace
			.Ray( InteractionRay, InteractionReach )
			.WithTag( "interactable" )
			.Run();

		var previouslyHovered = Hovered;

		if ( tr.Hit )
		{
			// There's no chance that GameObject wouldn't be a GameObject... right?
			Hovered = (GameObject)tr.Body.GameObject;
		}
		else
		{
			Hovered = null;
		}

		if ( previouslyHovered != Hovered )
		{
			if ( previouslyHovered is not null )
				Unhover( previouslyHovered );
			if ( Hovered is not null )
				Hover( Hovered );
		}

		if ( DebugDraw )
		{
			DoDebugDraw( tr );
		}
	}

	private void Unhover( GameObject go )
	{
		if ( go == null )
			return;

		if ( go.Components.TryGet<HighlightOutline>( out var outline ) )
		{
			outline.Destroy();
		}
		go.Tags.Remove( "hovered" );
	}

	private void Hover( GameObject go )
	{
		var outline = go.Components.Get<HighlightOutline>();
		if ( outline is null )
		{
			outline = go.Components.Create<HighlightOutline>();
		}
		outline.Enabled = true;
		go.Tags.Add( "hovered" );
		HoveredInfoPanel.Instance.Hovered = go;
		var affordances = GetAffordancesFromHovered();
		foreach ( var affordance in affordances )
		{
			InputGlyphsPanel.Instance.AddGlyph( new InputGlyphData
			{
				ActionName = affordance.ActionButton,
				DisplayText = affordance.AffordanceText,
				RemovalPredicate = () => !go.Tags.Has( "hovered" ) || !affordance.Enabled
			} );
		}
	}

	public IEnumerable<AffordanceComponent> GetAffordancesFromHovered()
	{
		if ( Hovered?.IsValid != true )
			return null;

		var affordances = Hovered.Components.GetAll<AffordanceComponent>( FindMode.EnabledInSelf );
		var addedAffordances = new List<AffordanceComponent>();
		foreach ( var affordance in affordances )
		{
			// If multiple components use the same action button on the same GameObject, only add the first one.
			if ( addedAffordances.Any( a => a.ActionButton == affordance.ActionButton ) )
				continue;

			addedAffordances.Add( affordance );
		}
		return addedAffordances;
	}

	private void DoDebugDraw( PhysicsTraceResult tr )
	{
		Gizmo.Draw.Color = tr.Hit ? Color.Red : Color.Blue;
		Gizmo.Draw.Line( InteractionRay.Position, tr.EndPosition );
		Gizmo.Draw.LineSphere( tr.StartPosition, 1f );
		Gizmo.Draw.LineSphere( tr.StartPosition.LerpTo( tr.EndPosition, 0.33f ), 1f );
		Gizmo.Draw.LineSphere( tr.StartPosition.LerpTo( tr.EndPosition, 0.66f ), 1f );
		if ( tr.Hit )
		{
			Gizmo.Draw.LineSphere( tr.EndPosition, 2f );
		}
	}
}
