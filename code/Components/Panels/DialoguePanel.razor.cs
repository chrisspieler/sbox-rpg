using Sandbox.UI;

namespace Sandbox;

public partial class DialoguePanel : PanelComponent
{
	public static DialoguePanel Instance { get; private set; }
	public static float DefaultCharacterDelay { get; set; } = 0.05f;
	public bool IsDialogueActive => CurrentCommand is not null;

	public string SpeakerName
	{
		get => _speakerName;
		set
		{
			_speakerName = value;

			if ( string.IsNullOrWhiteSpace( value ) )
				return;
		}
	}
	private string _speakerName = null;
	public int SpeakerFontSize => SpeakerName?.Length switch
	{
		< 14 => 48,
		< 20 => 36,
		< 26 => 30,
		_ => 24
	};
	public string DialogueFrameClass => string.IsNullOrWhiteSpace( SpeakerName ) ? "no-speaker" : "speaker";
	private Panel DialogueFrame { get; set; }
	private Panel NamePlate { get; set; }
	public bool LockPlayer { get; set; } = true;
	
	public DialoguePanel()
	{
		Instance = this;
	}

	public DialogueCommand CurrentCommand
	{
		get => _currentCommand;
		set
		{
			_currentCommand = value;

			if ( LockPlayer && _currentCommand is not null )
			{
				RpgPlayerController.Instance.BlockLook( this );
				RpgPlayerController.Instance.BlockMovement( this );
				return;
			}

			RpgPlayerController.Instance.UnblockLook( this );
			RpgPlayerController.Instance.UnblockMovement( this );
		}
	}
	private DialogueCommand _currentCommand;
	private List<DialogueCommand> _dialogueCommands = new();
	private int _currentCommandIndex = 0;

	protected override void OnUpdate()
	{
		if ( CurrentCommand is null && _currentCommandIndex < _dialogueCommands.Count )
		{
			CurrentCommand = _dialogueCommands[_currentCommandIndex];
		}

		// Process as many commands as will complete in a single tick, and
		// if the current command does not end, we'll execute it again next tick.
		while ( CurrentCommand?.Execute() == false )
		{
			CurrentCommand = null;
			_currentCommandIndex++;
			if ( _currentCommandIndex < _dialogueCommands.Count )
			{
				CurrentCommand = _dialogueCommands[_currentCommandIndex];
			}
		}
	}

	public void BeginDialogue( List<DialogueCommand> commands )
	{
		ClearAll();
		_dialogueCommands = commands;
	}

	public void PushCommands( IEnumerable<DialogueCommand> commands )
	{
		_dialogueCommands.AddRange( commands );
	}

	public void ClearAll()
	{
		SpeakerName = null;
		ClearDialogueFrame();
		CurrentCommand = null;
		_dialogueCommands.Clear();
		_currentCommandIndex = 0;
		LockPlayer = true;
		StateHasChanged();
	}

	public void ClearDialogueFrame()
	{
		DialogueFrame?.DeleteChildren( true );
	}

	public void PushDialogueFragment( Panel dialogueFragment )
	{
		DialogueFrame.AddChild( dialogueFragment );
		StateHasChanged();
	}

	protected override int BuildHash()
	{
		return HashCode.Combine( SpeakerName, CurrentCommand, _dialogueCommands.Count );
	}
}
