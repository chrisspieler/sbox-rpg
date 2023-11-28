namespace Sandbox;

public class DialogueBuilder
{
	public List<DialogueCommand> Commands { get; private set; } = new();

	public DialogueBuilder SetSpeaker( string speakerName, GameObject speakerGo = null )
	{
		Commands.Add( new DialogueSetSpeakerCommand()
		{
			SpeakerName = speakerName,
			SpeakerGo = speakerGo
		} );
		return this;
	}

	public DialogueBuilder AddBlock( string text, bool promptContinue = true )
	{
		Commands.Add( new DialoguePrintCommand()
		{
			Text = text
		} );
		if ( promptContinue )
		{
			Commands.Add( new DialoguePromptContinueCommand() );
		}
		Commands.Add( new DialogueClearCommand() );
		return this;
	}

	public void Begin()
	{
		DialoguePanel.Instance.BeginDialogue( Commands );
	}
}
