namespace Sandbox;

public class GaussianBlurTextureEffect : TextureEffectComponent
{
	private Texture _outputTex;

	public override Texture Apply( Texture texture )
	{
		if ( _outputTex == null || _outputTex.Size != texture.Size )
		{
			_outputTex?.Dispose();
			_outputTex = CreateTexture( texture.Size, $"{GameObject.Name}_GaussianBlurTexture" );
		}
		ProjectorShaders.DispatchGaussianBlur( texture, _outputTex );
		return _outputTex;
	}
}
