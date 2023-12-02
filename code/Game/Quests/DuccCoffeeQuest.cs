namespace Sandbox;

public class DuccCoffeeQuest : Quest
{
	public override string Id => "MQ01";
	public override string Name => "Coffee Run";

	public bool GaveDuccCoffee { get; private set; }
	public int CoffeeQuality { get; private set; } = -1;

	private Npc Ducc { get; set; }

	public static DialogueBuilder CoffeeReminderDialogue
		=> new DialogueBuilder()
			.SetSpeaker( "ducc" )
			.AddBlock( "There should be some fresh coffee in the break area." )
			.AddBlock( "Make a right at the hallway, then turn left at the end. You can't miss it." );

	protected override void OnBegin()
	{
		AddObjective( new Objective("Bring ducc a coffee", () => GaveDuccCoffee) );
		Ducc = Npc.Get( "ducc" );
		if ( Ducc == null )
		{
			Log.Error( "Ducc not found!" );
			return;
		}
		else
		{
			Log.Info( "Found ducc" );
		}

		Ducc.SetHoldEnabled( true );
		Ducc.HeldObjectChanged += HeldObjectChanged;
	}

	private void HeldObjectChanged( object sender, GameObject heldObject )
	{
		if ( heldObject?.IsValid != true )
			return;

		var isCoffee = heldObject.Tags.Has( "coffee" );
		if ( !isCoffee )
		{
			DialogueBuilder.Create( "ducc" )
				.AddBlock( "This isn't coffee..." )
				.Begin();
			return;
		}

		var name = heldObject.Name.ToLower();
		if ( name.Contains( "fresh" ) )
		{
			DialogueBuilder.Create( "ducc" )
				.AddBlock( "There we go." )
				.Begin();
			CoffeeQuality = 2;
		}
		else if ( name.Contains( "tepid" ) )
		{
			DialogueBuilder.Create( "ducc" )
				.AddBlock( "Did you get lost on the way back or something?" )
				.AddBlock( "This coffee is room temperature." )
				.AddBlock( "..." )
				.Begin();
			CoffeeQuality = 1;
		}
		else
		{
			DialogueBuilder.Create( "ducc" )
				.AddBlock( "This is crap." )
				.AddBlock( "Smell that - it's rancid!" )
				.AddBlock( "...well, I guess we'll just have to move on." )
				.Begin();
			CoffeeQuality = 0;
		}
		GaveDuccCoffee = true;
		if ( CoffeeQuality >= 1 )
		{
			Complete();
		}
		else
		{
			Fail();
		}
		return;
	}


}
