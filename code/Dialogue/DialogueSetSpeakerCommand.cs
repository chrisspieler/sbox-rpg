namespace Sandbox;

public class DialogueSetSpeakerCommand : DialogueCommand
{
	public string SpeakerName { get; set; }
	public GameObject SpeakerGo { get; set; }

	public override bool Execute()
	{
		DialoguePanel.Instance.SpeakerName = SpeakerName;
		if ( SpeakerGo is not null )
		{
			var displayNameComponent = SpeakerGo.GetOrAddComponent<DisplayNameComponent>();
			displayNameComponent.Name = SpeakerName;
		}
		return false;
	}
}
