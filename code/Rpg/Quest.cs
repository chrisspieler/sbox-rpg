using Sandbox.Diagnostics;

namespace Sandbox;

public abstract partial class Quest
{
	public enum QuestState
	{
		NotStarted,
		InProgress,
		Completed,
		Failed
	}

	public abstract string Id { get; }
	public abstract string Name { get; }
	public QuestState State { get; protected set; } = QuestState.NotStarted;
	protected List<Objective> CompletedObjectives { get; } = new();
	protected List<Objective> ActiveObjectives { get; } = new();

	protected Quest()
	{
		Assert.False( _questDb.ContainsKey( Id ), $"Duplicate quest ID created: {Id}" );
		_questDb[Id] = this;
		Event.Register( this );
	}

	~Quest()
	{
		Event.Unregister( this );
	}

	[Event( "game.tick" )]
	private void OnTick()
	{
		var objectives = ActiveObjectives.ToArray();
		foreach( var objective in objectives )
		{
			if ( objective.IsComplete() )
			{
				CompletedObjectives.Add( objective );
				ActiveObjectives.Remove( objective );
			}
		}
	}

	public IEnumerable<Objective> GetCompletedObjectives() => CompletedObjectives;
	public IEnumerable<Objective> GetActiveObjectives() => ActiveObjectives;
	public void AddObjective( Objective objective ) => ActiveObjectives.Add( objective );
}
