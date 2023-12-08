namespace Sandbox;

/// <summary>
/// Defines a certain interaction that a player may perform on a GameObject.
/// The action key and some text are displayed to the player when they are
/// able to act.
/// </summary>
public class AffordanceComponent : Component
{
	public virtual string AffordanceText => "Use";
	public virtual string ActionButton => "use";

	public virtual void DoInteract( GameObject user, HandState state = null ) { }

	protected override void OnEnabled()
	{
		base.OnEnabled();

		GameObject.Tags.Add( "interactable" );
	}

	protected override void OnDisabled()
	{
		base.OnDisabled();

		var otherAffordances = GameObject
			.Components
			.GetAll<AffordanceComponent>( FindMode.EnabledInSelf )
			.Where( c => c != this );
		if ( !otherAffordances.Any() )
			GameObject.Tags.Remove( "interactable" );
	}
}


