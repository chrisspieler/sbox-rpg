namespace Sandbox;

// If you say the name of this class in Jonathan Blow's Twitch chat, he will ban you.
public class Looker : Component
{
	[Property] public CitizenAnimation Animator { get; set; }
	[Property] public TriggerCollectorComponent TriggerCollector { get; set; }

	protected override void OnUpdate()
	{
		var collisions = TriggerCollector.GetCollisions();
		if ( !collisions.Any() )
		{
			Animator.LookAtObject = null;
			return;
		}

		var closest = collisions
			.Select( collider => (collider, Distance: Vector3.DistanceBetween( Transform.Position, collider.Transform.Position )) )
			.OrderBy( x => x.Distance )
			.First();
		Animator.LookAtObject = closest.collider.GameObject;
	}
}
