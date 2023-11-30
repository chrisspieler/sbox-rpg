namespace Sandbox;

public class GameManagerComponent : BaseComponent
{
	protected override void OnAwake()
	{
		Event.Run( "game.start" );
	}

	protected override void OnDestroy()
	{
		Event.Run( "game.stop" );
	}

	protected override void OnUpdate()
	{
		Event.Run( "game.tick" );
	}
}
