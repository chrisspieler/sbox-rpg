namespace Sandbox;

public class TriggerCollectorComponent : Component, Component.ITriggerListener
{
	[Property] public bool DebugDraw { get; set; }
	[Property] public TagSet IncludeAllTags { get; set; } = new();
	[Property] public TagSet ExcludeAllTags { get; set; } = new();


	public event EventHandler<Collider> TriggerEnter;
	public event EventHandler<Collider> TriggerExit;
	public IEnumerable<Collider> GetCollisions() => Collisions;
	protected HashSet<Collider> Collisions { get; } = new();

	protected override void OnUpdate()
	{
		foreach ( var collider in Collisions )
		{
			if ( !HasValidTags( collider ) )
			{
				OnTriggerExit( collider );
			}
		}

		if ( !DebugDraw )
			return;

		var collisionCount = Collisions.Count;
		var gizmoDrawPosition = Transform.Position + Vector3.Up * 40f;
		var gizmoTx = Transform.World.WithPosition( gizmoDrawPosition );
		Gizmo.Draw.Color = Color.White;
		Gizmo.Draw.IgnoreDepth = false;
		Gizmo.Draw.Text( $"Touch Count: {Collisions.Count}", gizmoTx);
	}

	public void OnTriggerEnter( Collider other )
	{
		if ( !HasValidTags( other ) )
			return;

		Collisions.Add( other );
		TriggerEnter?.Invoke( this, other );
	}

	public void OnTriggerExit( Collider other )
	{
		Collisions.Remove( other );
		TriggerExit?.Invoke( this, other );
	}

	private bool HasValidTags( Collider other )
	{
		foreach ( var requiredTag in IncludeAllTags.TryGetAll() )
		{
			if ( !other.GameObject.Tags.Has( requiredTag ) )
				return false;
		}
		foreach ( var forbiddenTag in ExcludeAllTags.TryGetAll() )
		{
			if ( other.GameObject.Tags.Has( forbiddenTag ) )
				return false;
		}
		return true;
	}
}
