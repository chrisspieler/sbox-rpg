namespace Sandbox;

public partial class Npc
{
	private static Dictionary<string, Npc> _npcDb = new();

	[Event( "game.stop" )]
	private static void OnStop()
	{
		_npcDb.Clear();
	}

	public static Npc Get( string id )
	{
		if ( _npcDb.TryGetValue( id, out var npc ) )
			return npc;

		return null;
	}
}
