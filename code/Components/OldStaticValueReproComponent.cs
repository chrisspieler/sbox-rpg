namespace Sandbox;

public class OldStaticValueReproComponent : Component
{
	private static int _staticCounter = 0;
	private static Dictionary<float, object> _staticDictionary = new();

	protected override void OnStart()
	{
		_staticCounter++;
		Log.Info( $"Static counter is {_staticCounter}" );
		_staticDictionary[Time.Now] = new();
		Log.Info( $"Static dictionary has {_staticDictionary.Count} entries" );
	}

	[Event( "game.stop" )]
	public static void OnGameStop()
	{
		_staticCounter = 0;
		_staticDictionary.Clear();
	}
}
