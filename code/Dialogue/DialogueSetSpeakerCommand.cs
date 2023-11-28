namespace Sandbox;

public class DialogueSetSpeakerCommand : DialogueCommand
{
	public string SpeakerName { get; set; }

	public override bool Execute()
	{
		DialoguePanel.Instance.SpeakerName = SpeakerName;
		return false;
	}
}
