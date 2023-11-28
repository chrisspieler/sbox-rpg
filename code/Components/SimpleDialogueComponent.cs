namespace Sandbox;

public class SimpleDialogueComponent : AffordanceComponent
{
	[Property] public string DialogueStartPrompt { get; set; } = "Examine";
	[Property] public string DialogueText { get; set; } = "Doesn't look like anything to me.";
	[Property] public bool DisableAfterDisplay { get; set; } = false;
	[Property] public bool AdvanceAfterDisplay { get; set; } = false;

	public override string AffordanceText => DialogueStartPrompt;
	public int InteractionCounter { get; private set; }

	public override void DoInteract( GameObject user, HandState state = null )
	{
		RunDialogue();
	}

	public void RunDialogue()
	{
		InteractionCounter++;

		var builder = new DialogueBuilder();
		if ( GameObject.TryGetComponent<DisplayNameComponent>( out var nameComponent ) )
		{
			builder.SetSpeaker( nameComponent.Name );
		}
		else if ( !DialoguePanel.Instance.IsDialogueActive )
		{
			builder.SetSpeaker( null );
		}
		builder.AddBlock( DialogueText );
		if ( DialoguePanel.Instance.IsDialogueActive )
		{
			DialoguePanel.Instance.PushCommands( builder.Commands );
		}
		else
		{
			DialoguePanel.Instance.PushCommands( builder.Commands );
		}

		Enabled = !DisableAfterDisplay;

		if ( !AdvanceAfterDisplay )
			return;

		var nextDialogue = GetNextDialogue();
		nextDialogue?.RunDialogue();
	}

	private SimpleDialogueComponent GetNextDialogue()
	{
		var dialogueComponents = GameObject.GetComponents<SimpleDialogueComponent>().ToArray();
		var index = Array.IndexOf( dialogueComponents, this );
		// If there's a next dialogue component, return it.
		if ( index < dialogueComponents.Length - 1 )
			return dialogueComponents[index + 1];

		return null;
	}
}
