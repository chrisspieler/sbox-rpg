namespace Sandbox;

public class SimpleDialogueComponent : AffordanceComponent
{
	[Property] public string DialogueStartPrompt { get; set; } = "Examine";
	[Property] public string DialogueText { get; set; } = "Doesn't look like anything to me.";

	public override string AffordanceText => DialogueStartPrompt;

	public override void DoInteract( GameObject user, HandState state = null )
	{
		var builder = new DialogueBuilder();
		if ( GameObject.TryGetComponent<DisplayNameComponent>( out var nameComponent ) )
		{
			builder.SetSpeaker( nameComponent.Name );
		}
		builder.AddBlock( DialogueText );
		DialoguePanel.Instance.BeginDialogue( builder.Commands );
	}
}
