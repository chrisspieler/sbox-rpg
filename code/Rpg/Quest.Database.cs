namespace Sandbox;

public abstract partial class Quest
{
	private static Dictionary<string, Quest> _questDb = new();

	[Event( "game.start" )]
	private static void OnStart()
	{
		if ( _questDb.Any() )
			return;

		var quests = TypeLibrary.GetTypes<Quest>()
			.Where( t => !t.IsAbstract );
		foreach( var quest in quests )
		{
			TypeLibrary.Create<Quest>( quest.Identity );
		}
	}

	[Event( "game.stop" )]
	private static void OnStop()
	{
		_questDb.Clear();
	}

	public static Quest Begin<T>() where T : Quest, new()
	{
		var quest = _questDb
			.Values
			.OfType<T>()
			.FirstOrDefault() ?? new T();
		quest.State = QuestState.InProgress;
		return quest;
	}

	public static bool HasBegun<T>() where T : Quest
		=> _questDb.Values.OfType<T>().Any( x => x.State != QuestState.NotStarted );

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
