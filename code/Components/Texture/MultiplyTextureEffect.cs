namespace Sandbox;

public class MultiplyTextureEffect : TextureEffectComponent
{
	[Property] public Texture MultiplicandTexture { get; set; }

	private Texture _outputTex;
	public override Texture Apply( Texture texture )
	{
		if ( _outputTex == null || _outputTex.Size != texture.Size )
		{
			_outputTex?.Dispose();
			_outputTex = CreateTexture( texture.Size, $"{GameObject.Name}_MultiplyTexture" );
		}
		ProjectorShaders.DispatchMultiply( texture, MultiplicandTexture, _outputTex );
		return _outputTex;
	}
}
