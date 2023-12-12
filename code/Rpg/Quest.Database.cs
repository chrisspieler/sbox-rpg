namespace Sandbox;

public abstract partial class Quest
{
	private static Dictionary<string, Quest> _questDb = new();

	public static void InitializeQuests()
	{
		_questDb.Clear();

		var quests = TypeLibrary.GetTypes<Quest>()
			.Where( t => !t.IsAbstract );
		foreach( var quest in quests )
		{
			TypeLibrary.Create<Quest>( quest.Identity );
		}
	}

	public static Quest Get<T> () where T : Quest
		=> _questDb.Values.OfType<T>().FirstOrDefault();

	public static Quest Get( string id )
	{
		if ( _questDb.TryGetValue( id, out var quest ) )
			return quest;

		return null;
	}

	public static IEnumerable<Quest> GetAll()
	{
		return _questDb.Values;
	}
}
