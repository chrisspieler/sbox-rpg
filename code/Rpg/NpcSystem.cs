namespace Sandbox.Rpg
{
	public class NpcSystem : GameObjectSystem
	{
		public NpcSystem( Scene scene ) : base( scene )
		{
			Npc.InitializeNpcs();
		}
	}
}
