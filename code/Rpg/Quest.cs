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
	}

	public void Update()
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

	public void Begin()
	{
		Assert.True( State == QuestState.NotStarted, $"Quest {Id} is already started" );
		State = QuestState.InProgress;
		OnBegin();
	}

	public void Complete()
	{
		Assert.True( State == QuestState.InProgress, $"Quest {Id} is not in progress" );
		State = QuestState.Completed;
		OnEnd();
	}

	public void Fail()
	{
		Assert.True( State == QuestState.InProgress, $"Quest {Id} is not in progress" );
		State = QuestState.Failed;
		OnEnd();
	}

	protected virtual void OnBegin() { }
	protected virtual void OnEnd() { }

	public IEnumerable<Objective> GetCompletedObjectives() => CompletedObjectives;
	public IEnumerable<Objective> GetActiveObjectives() => ActiveObjectives;
	public void AddObjective( Objective objective ) => ActiveObjectives.Add( objective );
}
