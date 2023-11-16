namespace Sandbox;

public class InteractableComponent : BaseComponent
{
	[Property] public string InteractionText { get; set; } = "Use";

	public event EventHandler<GameObject> OnInteract;

	public override void OnStart()
	{
		if ( !GameObject.Tags.Has( "interactable" ) )
			GameObject.Tags.Add( "interactable" );
	}

	public void Interact( GameObject user )
	{
		OnInteract?.Invoke( this, user );
	}
}
