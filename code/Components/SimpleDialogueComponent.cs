namespace Sandbox;

public class SimpleDialogueComponent : AffordanceComponent
{
	[Property] public string DialogueStartPrompt { get; set; } = "Examine";
	[Property] public string DialogueText { get; set; } = "Doesn't look like anything to me.";

	public override string AffordanceText => DialogueStartPrompt;

	public override void DoInteract( GameObject user, HandState state = null )
	{
		var commands = new List<DialogueCommand>();
		if ( GameObject.TryGetComponent<DisplayNameComponent>( out var nameComponent ) )
		{
			commands.Add( new DialogueSetSpeakerCommand() { SpeakerName = nameComponent.Name } );
		}
		commands.Add( new DialoguePrintCommand() { Text = DialogueText } );
		commands.Add( new DialoguePromptContinueCommand() );
		DialoguePanel.Instance.BeginDialogue( commands );
	}
}
