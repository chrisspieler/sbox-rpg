namespace Sandbox;

public partial class Npc
{
	private static Dictionary<string, Npc> _npcDb = new();

	public static void InitializeNpcs()
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
