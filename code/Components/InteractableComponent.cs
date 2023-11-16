namespace Sandbox;

public class InteractableComponent : BaseComponent
{
	[Property] public string InteractionText { get; set; } = "Use";

	public event EventHandler<GameObject> OnInteract;

	public void Interact( GameObject user )
	{
		OnInteract?.Invoke( this, user );
	}
}
