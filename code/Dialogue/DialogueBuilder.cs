﻿namespace Sandbox;

public class DialogueBuilder
{
	public List<DialogueCommand> Commands { get; private set; } = new();

	public static DialogueBuilder Create( string speaker = null )
	{
		return new DialogueBuilder().SetSpeaker( speaker );
	}

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
			Action = () => Quest.Get<T>().Begin()
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

	public DialogueBuilder UnlockPlayer()
	{
		// For most dialogue, we don't want the player to move around, so we
		// lock them in place by default.
		Commands.Add( new DialogueActionCommand()
		{
			Action = () => DialoguePanel.Instance.LockPlayer = false
		} );
		return this;
	}

	public void Begin()
	{
		DialoguePanel.Instance.BeginDialogue( Commands );
	}
}
