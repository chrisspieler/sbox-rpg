namespace Sandbox;

public class SimpleDialogueComponent : AffordanceComponent
{
	[Property] public string SpeakerName { get; set; } = null;
	[Property] public string DialogueStartPrompt { get; set; } = "Examine";
	[Property] public string DialogueText { get; set; } = "Doesn't look like anything to me.";

	public override string AffordanceText => DialogueStartPrompt;

	public override void DoInteract( GameObject user, HandState state = null )
	{
		var setSpeaker = new DialogueSetSpeakerCommand() { SpeakerName = SpeakerName };
		var print = new DialoguePrintCommand() { Text = DialogueText };
		var promptContinue = new DialoguePromptContinueCommand();
		var commands = new List<DialogueCommand>() { setSpeaker, print, promptContinue };
		DialoguePanel.Instance.BeginDialogue( commands );
	}
}
