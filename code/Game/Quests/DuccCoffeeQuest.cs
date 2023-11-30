namespace Sandbox;

public class DuccCoffeeQuest : Quest
{
	public override string Id => "MQ01";
	public override string Name => "Coffee Run";

	public static DialogueBuilder CoffeeReminderDialogue
		=> new DialogueBuilder()
			.SetSpeaker( "ducc" )
			.AddBlock( "There should be some fresh coffee in the break area." )
			.AddBlock( "Make a right at the hallway, then turn left at the end. You can't miss it." );
}
