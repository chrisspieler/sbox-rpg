namespace Sandbox;

public class QuestSystem : GameObjectSystem
{
	public QuestSystem( Scene scene ) : base( scene )
	{
		Quest.InitializeQuests();
	}
}
