using Sandbox.UI;

namespace Sandbox;

public sealed class TexturePanel : Component
{
	[Property] public DynamicTextureComponent TextureTarget { get; set; }
	[Property] public PanelComponent PanelSource { get; set; }

	private SceneCustomObject _renderObject;
	private RootPanel _rootPanel;
	private Texture _texture;

	protected override void OnStart()
	{
		EnsureRootPanel();
		_renderObject = new SceneCustomObject( Scene.SceneWorld );
		_renderObject.RenderOverride = Render;
		if ( _rootPanel is not null )
		{
			CreateInputTexture();
		}
	}

	private void EnsureRootPanel()
	{
		if ( _rootPanel != null )
			return;

		_rootPanel = PanelSource?.Panel?.FindRootPanel();
		if ( _rootPanel is null )
			return;
	}
	
	private void CreateInputTexture()
	{
		_texture = Texture.CreateRenderTarget()
				 .WithSize( _rootPanel.PanelBounds.Size )
				 .WithScreenFormat()
				 .WithScreenMultiSample()
				 .Create();
		TextureTarget.SetTexture( _texture );
	}

	private void Render( SceneObject sceneObject )
	{
		EnsureRootPanel();

		if ( _rootPanel is null )
			return;
		
		if ( TextureTarget.InputTexture is null )
		{
			CreateInputTexture();
			return;
		}
		Graphics.RenderTarget = RenderTarget.From( TextureTarget.InputTexture );
		Graphics.Attributes.SetCombo( "D_WORLDPANEL", 0 );
		Graphics.Viewport = new Rect( 0, _rootPanel.PanelBounds.Size );
		Graphics.Clear( Color.White );

		_rootPanel.RenderManual( 1f );

		Graphics.RenderTarget = null;
	}

	private class TestPanel : RootPanel
	{
		public override void DrawBackground( ref RenderState state )
		{
			Span<Vertex> vertices = new Vertex[]
			{
				new Vertex( new Vector3( 0, 0 ), new Vector4( 0, 0, 0, 0 ), new Color32(255, 0, 0 ) ),
				new Vertex( new Vector3( 500, 0 ), new Vector4( 0, 0, 0, 0 ), new Color32(0, 255, 0 ) ),
				new Vertex( new Vector3( 500, 500 ), new Vector4( 0, 0, 0, 0 ), new Color32(0, 0, 255 ) ),
			};

			var attribs = new RenderAttributes();
			attribs.Set( "Texture", Texture.White );

			Graphics.Draw( vertices, 3, Material.UI.Basic, attribs, Graphics.PrimitiveType.Triangles );
		}
	}
}
