namespace Sandbox;

public abstract class DialogueCommand
{
	/// <summary>
	/// Process a tick of the dialogue command. Returns true if the command is still running, 
	/// or false if it is finished.
	/// </summary>
	public abstract bool Execute();
}
