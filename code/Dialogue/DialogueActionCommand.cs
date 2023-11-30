namespace Sandbox;

public class DialogueActionCommand : DialogueCommand
{
	public Action Action { get; set; }

	public override bool Execute()
	{
		Action?.Invoke();
		return false;
	}
}
