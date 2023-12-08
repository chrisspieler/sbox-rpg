using Sandbox.Utility;

namespace Sandbox;

public class FootstepSoundPlayerComponent : Component
{
	[Property] public SkinnedModelRenderer Source 
	{
		get => _source;
		set
		{
			if (_source != null )
			{
				_source.OnFootstepEvent -= OnFootstepEvent;
			}
			_source = value;
			if (value != null )
			{
				value.OnFootstepEvent += OnFootstepEvent;
			}
		}
	}
	private SkinnedModelRenderer _source;
	[Property] public bool DebugDraw { get; set; }
	private CircularBuffer<SceneModel.FootstepEvent> _pastFootsteps = new( 5 );
	private CircularBuffer<PhysicsTraceResult> _pastTraces = new( 5 );

	protected override void OnStart()
	{
		if ( Source != null )
		{
			Source.OnFootstepEvent += OnFootstepEvent;
		}
	}

	protected override void OnUpdate()
	{
		if ( DebugDraw )
		{
			DebugDrawFootText();
			DebugDrawTraces();
		}
	}

	private void OnFootstepEvent( SceneModel.FootstepEvent footstepEvent )
	{
		var cc = Components.Get<CharacterController>();
		if ( cc is not null )
		{
			footstepEvent.Volume = MathX.Remap( cc.Velocity.Length, 0f, 300f, 0.2f, 1f );
		}

		var traceStart = footstepEvent.Transform.Position + Vector3.Up * 10f;
		var traceEnd = footstepEvent.Transform.Position - Vector3.Up * 10f;
		var tracer = Scene.PhysicsWorld.Trace;
		var tr = tracer.Ray(traceStart, traceEnd)
			.Run();
		
		if ( tr.Hit )
		{
			tr.Surface.DoFootstep( tr, footstepEvent.FootId, footstepEvent.Volume );
		}

		// Save some data for debug visualization.
		_pastFootsteps.PushFront( footstepEvent );
		_pastTraces.PushFront( tr );
	}

	private void DebugDrawFootText( )
	{
		Gizmo.Draw.Color = Color.Yellow;
		foreach(var pastFootstep in _pastFootsteps )
		{
			var footName = pastFootstep.FootId == 0 ? "left" : "right";

			Gizmo.Draw.IgnoreDepth = true;
			Gizmo.Draw.Text( $"foot_{footName}: vol {pastFootstep.Volume}", pastFootstep.Transform );
		}
	}

	private void DebugDrawTraces()
	{
		foreach( var pastTrace in _pastTraces )
		{
			Gizmo.Draw.Color = pastTrace.Hit ? Color.Red : Color.Blue;
			Gizmo.Draw.IgnoreDepth = true;
			Gizmo.Draw.Line( pastTrace.StartPosition, pastTrace.EndPosition );
			if ( pastTrace.Hit )
			{
				Gizmo.Draw.LineSphere( new Sphere( pastTrace.HitPosition, 1f ) );
			}
		}
	}
}
