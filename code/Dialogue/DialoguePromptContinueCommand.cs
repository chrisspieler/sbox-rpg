namespace Sandbox;

public class DialoguePromptContinueCommand : DialogueCommand
{
	public string ContinueAction { get; set; } = "use";
	public override bool Execute()
	{
		return !Input.Pressed( ContinueAction );
	}
}
