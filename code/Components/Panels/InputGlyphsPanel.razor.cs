using System.Collections.Generic;

namespace Sandbox;

public partial class InputGlyphsPanel
{
	public static InputGlyphsPanel Instance { get; private set; }
	private Dictionary<string, InputGlyphData> Glyphs { get; set; } = new();

	public InputGlyphsPanel()
	{
		Instance = this;
	}

	public void AddGlyph( InputGlyphData data )
	{
		Glyphs[data.ActionName] = data;
		Panel.StateHasChanged();
	}

	public override void Update()
	{
		var glyphs = Glyphs.Values.ToList();
		foreach(var glyph in glyphs )
		{
			if ( glyph.RemovalPredicate?.Invoke() == true )
			{
				Glyphs.Remove( glyph.ActionName );
			}
		}
		Panel.StateHasChanged();
	}

	private void DrawGlyph( InputGlyphData data )
	{
		var glyphControl = new InputGlyphControl();
		glyphControl.GlyphData = data;
		Panel.AddChild( glyphControl );
	}
}

public struct InputGlyphData
{
	public string ActionName { get; set; }
	public string DisplayText { get; set; }
	public Func<bool> RemovalPredicate { get; set; }
}
