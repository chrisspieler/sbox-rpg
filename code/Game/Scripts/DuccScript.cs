namespace Sandbox;

public class DuccScript : AffordanceComponent
{
	public override string AffordanceText => "Talk";

	private int _interactionCounter;

	protected override void OnStart()
	{
		var onboardingQuest = Quest.Begin<OnboardingQuest>();
		var firstObjective = new Objective()
		{
			Description = "Speak to ducc, your new manager.",
			IsComplete = () => _interactionCounter >= 1
		};
		onboardingQuest.AddObjective( firstObjective );
	}

	public override void DoInteract( GameObject user, HandState state = null )
	{
		_interactionCounter++;

		// Reminder to get coffee.
		if ( Quest.HasBegun<DuccCoffeeQuest>() )
		{
			DuccCoffeeQuest.CoffeeReminderDialogue.Begin();
			return;
		}
		// First interaction.
		if ( _interactionCounter == 1 )
		{
			GreetingDialogue.Begin();
			return;
		}

		// Flavor text after the coffee quest.
		FlavorDialogue.Begin();
		return;
	}

	private DialogueBuilder GreetingDialogue
		=> new DialogueBuilder()
			.AddBlock( "You must be our new hire." )
			.SetSpeaker( "ducc", GameObject )
			.AddBlock( "I'm ducc. Welcome aboard." )
			.PushObjective<OnboardingQuest>( new Objective( "Follow ducc's orders." ) )
			.AddBlock( "I have an important task for you, yes." )
			.AddBlock( "You will be fetching me coffee." )
		    .BeginQuest<DuccCoffeeQuest>()
			.PushObjective<DuccCoffeeQuest>( new Objective( "Bring ducc some coffee." ))
			.AddBlock( "Now, get to it!" );

	private DialogueBuilder FlavorDialogue
		=> new DialogueBuilder()
			.SetSpeaker( "ducc" )
			.AddBlock( "What is it that you need from me?" )
			.AddBlock( "Some bread crumbs, perhaps?" );
}
