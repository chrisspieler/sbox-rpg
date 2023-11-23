namespace Sandbox;

/// <summary>
/// Defines a certain interaction that a player may perform on a GameObject.
/// The action key and some text are displayed to the player when they are
/// able to act.
/// </summary>
public class AffordanceComponent : BaseComponent
{
	public virtual string AffordanceText => "Use";
	public virtual string ActionButton => "use";

	public virtual void DoInteract( GameObject user, HandState state = null ) { }

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


