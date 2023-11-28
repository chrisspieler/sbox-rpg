using Sandbox.UI;

namespace Sandbox;

public partial class DialoguePanel : PanelComponent
{
	public static DialoguePanel Instance { get; private set; }
	public static float DefaultCharacterDelay { get; set; } = 0.09f;
	public bool IsDialogueActive => _currentCommand is not null;

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

	public DialoguePanel()
	{
		Instance = this;
	}

	public DialogueCommand _currentCommand { get; private set; }
	private List<DialogueCommand> _dialogueCommands = new();
	private int _currentCommandIndex = 0;

	public override void Update()
	{
		if ( _currentCommand is null && _currentCommandIndex < _dialogueCommands.Count )
		{
			_currentCommand = _dialogueCommands[_currentCommandIndex];
		}

		// Process as many commands as will complete in a single tick, and
		// if the current command does not end, we'll execute it again next tick.
		while ( _currentCommand?.Execute() == false )
		{
			_currentCommand = null;
			_currentCommandIndex++;
			if ( _currentCommandIndex < _dialogueCommands.Count )
			{
				_currentCommand = _dialogueCommands[_currentCommandIndex];
			}
		}
	}

	public void BeginDialogue( List<DialogueCommand> commands )
	{
		ClearAll();
		_dialogueCommands = commands;
	}

	public void ClearAll()
	{
		SpeakerName = null;
		ClearDialogueFrame();
		_currentCommand = null;
		_dialogueCommands.Clear();
		_currentCommandIndex = 0;
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
		return HashCode.Combine( SpeakerName, _currentCommand );
	}
}
