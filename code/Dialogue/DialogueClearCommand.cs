namespace Sandbox;

public class DialogueClearCommand : DialogueCommand
{
	public override bool Execute()
	{
		DialoguePanel.Instance.ClearDialogueFrame();
		return false;
	}
}
