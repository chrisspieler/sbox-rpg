namespace Sandbox;

public class QuestDebugOverlayComponent : Component
{
	private const int TEXT_START_Y = 10;
	private const int TEXT_LINE_HEIGHT = 20;
	private const int TEXT_INDENT = 30;
	private const int NOT_STARTED_X = 50;
	private const int IN_PROGRESS_X = 350;
	private const int COMPLETED_X = 650;
	private const int FAILED_X = 950;

	protected override void OnUpdate()
	{
		Gizmo.Draw.Color = Color.Yellow;
		Gizmo.Draw.ScreenText( "QUEST DEBUG OVERLAY", new Vector2( NOT_STARTED_X, TEXT_START_Y ), flags: TextFlag.Left );
		var categoryY = TEXT_START_Y + TEXT_LINE_HEIGHT;
		Gizmo.Draw.ScreenText( "NOT STARTED", new Vector2( NOT_STARTED_X, categoryY ), flags: TextFlag.Left );
		Gizmo.Draw.ScreenText( "IN PROGRESS", new Vector2( IN_PROGRESS_X, categoryY ), flags: TextFlag.Left );
		Gizmo.Draw.ScreenText( "COMPLETED", new Vector2( COMPLETED_X, categoryY ), flags: TextFlag.Left );
		Gizmo.Draw.ScreenText( "FAILED", new Vector2( FAILED_X, categoryY ), flags: TextFlag.Left );
		var notStartedY = categoryY + TEXT_LINE_HEIGHT;
		var inProgressY = categoryY + TEXT_LINE_HEIGHT;
		var completedY = categoryY + TEXT_LINE_HEIGHT;
		var failedY = categoryY + TEXT_LINE_HEIGHT;
		var quests = Quest.GetAll().OrderBy( q => q.Id ).ToArray();
		for( int i = 0; i < quests.Length; i++)
		{
			var quest = quests[i];
			var startY = quest.State switch
			{
				Quest.QuestState.NotStarted => notStartedY,
				Quest.QuestState.InProgress => inProgressY,
				Quest.QuestState.Completed => completedY,
				Quest.QuestState.Failed => failedY,
				_ => throw new NotImplementedException()
			};
			var newY = DrawQuest( quests[i], startY );
			switch ( quest.State )
			{
				case Quest.QuestState.NotStarted:
					notStartedY = newY;
					break;
				case Quest.QuestState.InProgress:
					inProgressY = newY;
					break;
				case Quest.QuestState.Completed:
					completedY = newY;
					break;
				case Quest.QuestState.Failed:
					failedY = newY;
					break;
				default:
					throw new NotImplementedException();
			}
		}
	}

	private int DrawQuest( Quest quest, int y )
	{
		var startX = quest.State switch
		{
			Quest.QuestState.NotStarted => NOT_STARTED_X,
			Quest.QuestState.InProgress => IN_PROGRESS_X,
			Quest.QuestState.Completed => COMPLETED_X,
			Quest.QuestState.Failed => FAILED_X,
			_ => throw new NotImplementedException()
		};
		Gizmo.Draw.ScreenText( $"({quest.Id}) {quest.Name}", new Vector2( startX, y ), flags: TextFlag.Left );
		y += TEXT_LINE_HEIGHT;
		Gizmo.Draw.Color = Color.Gray;
		foreach( var objective in quest.GetCompletedObjectives() )
		{
			Gizmo.Draw.ScreenText( objective.Description, new Vector2( startX + TEXT_INDENT, y ), flags: TextFlag.Left );
			y += TEXT_LINE_HEIGHT;
		}
		Gizmo.Draw.Color = Color.White;
		foreach( var objective in quest.GetActiveObjectives() )
		{
			Gizmo.Draw.ScreenText( objective.Description, new Vector2( startX + TEXT_INDENT, y ), flags: TextFlag.Left );
			y += TEXT_LINE_HEIGHT;
		}
		Gizmo.Draw.Color = Color.Yellow;
		return y;
	}
}
