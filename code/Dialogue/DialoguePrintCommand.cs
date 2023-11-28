using Sandbox.UI;

namespace Sandbox;

public class DialoguePrintCommand : DialogueCommand
{
	/// <summary>
	/// A factor that will be applied to the text speed.
	/// </summary>
	public float DelayFactor { get; set; } = 1f;
	public float CharacterDelay => DialoguePanel.DefaultCharacterDelay 
		* DelayFactor 
		/ (Input.Down( SpeedUpAction ) ? SpeedUpFactor : 1f);
	public string SpeedUpAction { get; set; } = "use";
	public float SpeedUpFactor { get; set; } = 2.5f;

	public string Text { get; set; }
	private Label _dialogueFragment;
	private int _currentCharacter = 0;
	private RealTimeUntil _nextCharacter;

	public DialoguePrintCommand()
	{
		_nextCharacter = CharacterDelay;
	}

	public override bool Execute()
	{
		// If we're at the end of the text, we're done.
		if ( _currentCharacter >= Text.Length )
			return false;

		// If we're not ready to print the next character, we're not done.
		if ( !_nextCharacter )
			return true;

		if ( _dialogueFragment is null )
		{
			_dialogueFragment = new Label();
			DialoguePanel.Instance.PushDialogueFragment( _dialogueFragment );
		}

		_dialogueFragment.Text = Text.Substring(0, _currentCharacter + 1);
		_dialogueFragment.StateHasChanged();
		_currentCharacter++;
		if ( _currentCharacter < Text.Length )
			_nextCharacter = GetDelayForChar( Text[_currentCharacter - 1] ) ;
		return true;
	}

	private float GetDelayForChar( char c )
	{
		var specialDelayFactor = c switch
		{
			'!' or '?' or '.' => 5f,
			',' => 3.5f,
			' ' or '\t' => 0f,
			_ => 1f
		};
		return CharacterDelay * specialDelayFactor;
	}
}
