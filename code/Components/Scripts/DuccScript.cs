namespace Sandbox;

public class DuccScript : AffordanceComponent
{
	public override string AffordanceText => "Talk";

	private int _interactionCounter;
	public override void DoInteract( GameObject user, HandState state = null )
	{
		if ( _interactionCounter == 0 )
		{
			DoFirstDialogue();
		}
		else if ( _interactionCounter == 1 )
		{
			DoSecondDialogue();
		}
		else
		{
			new DialogueBuilder()
				.SetSpeaker( "ducc" )
				.AddBlock( "I could go for some coffee." )
				.SetSpeaker( null )
				.AddBlock( "ducc looks at you expectantly." )
				.Begin();
		}
		_interactionCounter++;
	}

	public void DoFirstDialogue()
	{
		new DialogueBuilder()
			.AddBlock( "You must be our new hire." )
			.SetSpeaker( "ducc", GameObject )
			.AddBlock( "I'm ducc. Welcome aboard." )
			.Begin();
	}

	public void DoSecondDialogue()
	{
		new DialogueBuilder()
			.SetSpeaker( "ducc" )
			.AddBlock( "What is it that you need from me?" )
			.AddBlock( "Some bread crumbs, perhaps?" )
			.Begin();
	}
}
