namespace Sandbox;

/// <summary>
/// Defines a certain interaction that a player may perform on a GameObject.
/// The action key and some text are displayed to the player when they are
/// able to act.
/// </summary>
public class AffordanceComponent : BaseComponent
{
	[Property] public virtual string AffordanceText { get; set; } = "Use";
	[Property] public virtual string ActionButton { get; set; } = "use";

	public virtual void DoInteract( GameObject user ) { }

	public override void OnEnabled()
	{
		base.OnEnabled();

		GameObject.Tags.Add( "interactable" );
	}

	public override void OnDisabled()
	{
		base.OnDisabled();

		var otherAffordances = GameObject
			.GetComponents<AffordanceComponent>()
			.Where( c => c != this );
		if ( !otherAffordances.Any() )
			GameObject.Tags.Remove( "interactable" );
	}
}
