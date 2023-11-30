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

	public DialogueBuilder AddAction( Action action )
	{
		Commands.Add( new DialogueActionCommand()
		{
			Action = action
		} );
		return this;
	}

	public DialogueBuilder BeginQuest<T>() where T : Quest, new()
	{
		Commands.Add( new DialogueActionCommand()
		{
			Action = () => Quest.Begin<T>()
		} );
		return this;
	}

	public DialogueBuilder PushObjective<T>( Objective objective)
		where T : Quest
	{
		var quest = Quest.Get<T>();
		Commands.Add( new DialogueActionCommand()
		{
			Action = () => quest.AddObjective( objective )
		} );
		return this;
	}

	public void Begin()
	{
		DialoguePanel.Instance.BeginDialogue( Commands );
	}
}
