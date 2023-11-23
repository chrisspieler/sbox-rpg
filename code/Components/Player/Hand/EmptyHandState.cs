namespace Sandbox;

public class EmptyHandState : HandState
{
	[Property] public bool DebugDraw { get; set; }

	public GameObject Hovered { get; private set; }

	public override void OnEnabled() => Initialize();
	public override void OnStart() => Initialize();

	private void Initialize()
	{
		if ( HandModel is not null )
			HandModel.Enabled = false;
	}

	public override void Update()
	{
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
		if ( go.TryGetComponent<HighlightOutline>( out var outline ) )
		{
			outline.Destroy();
		}
		go.Tags.Remove( "hovered" );
	}

	private void Hover( GameObject go )
	{
		var outline = go.GetComponent<HighlightOutline>();
		if ( outline is null )
		{
			outline = go.AddComponent<HighlightOutline>();
		}
		outline.Enabled = true;
		go.Tags.Add( "hovered" );
		HoveredInfoPanel.Instance.Hovered = go;
		var affordances = go.GetComponents<AffordanceComponent>();
		foreach ( var affordance in affordances )
		{
			InputGlyphsPanel.Instance.AddGlyph( new InputGlyphData
			{
				ActionName = affordance.ActionButton,
				DisplayText = affordance.AffordanceText,
				RemovalPredicate = () => !go.Tags.Has( "hovered" )
			} );
		}
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
