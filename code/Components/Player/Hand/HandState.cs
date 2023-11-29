namespace Sandbox;

public abstract class HandState : PlayerState
{
	/// <summary>
	/// The maximum distance that the ray used for interactions can reach.
	/// </summary>
	[Property] public float InteractionReach { get; set; } = 80f;
	/// <summary>
	/// A game object whose position and rotation shall be used to project a ray
	/// that shall be used for interacting with via this hand.
	/// </summary>
	[Property] public GameObject InteractionRaySource { get; set; }
	public SkinnedModelRenderer HandModel => _handModel ??= Components.Get<SkinnedModelRenderer>( FindMode.EverythingInSelfAndDescendants );
	private SkinnedModelRenderer _handModel;

	public Ray InteractionRay => new( 
						origin: InteractionRaySource.Transform.Position, 
						direction: InteractionRaySource.Transform.Rotation.Forward 
					);
}
