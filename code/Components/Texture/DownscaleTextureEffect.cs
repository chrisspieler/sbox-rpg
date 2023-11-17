namespace Sandbox;

public class DownscaleTextureEffect : TextureEffectComponent
{
	/// <summary>
	/// If MatchTexture is not set, the texture argument to Apply will be 
	/// downscaled by this factor.
	/// </summary>
	[Property, Range(0.05f, 0.95f, 0.05f)] public float ScaleFactor { get; set; } = 0.5f;
	/// <summary>
	/// If MatchTexture is set, the output texture of Apply will be the same 
	/// size as MatchTexture, overriding ScaleFactor.
	/// </summary>
	[Property] public Texture MatchTexture { get; set; }

	private Texture _downscaledTexture;

	public override Texture Apply( Texture texture )
	{

		var targetSize = MatchTexture?.Size ?? texture.Size * ScaleFactor; 
		if ( _downscaledTexture == null || _downscaledTexture.Size != targetSize )
		{
			_downscaledTexture?.Dispose();
			_downscaledTexture = CreateTexture( targetSize, $"{GameObject.Name}_DownscaledTexture" );
		}
		ProjectorShaders.DispatchDownscale( texture, _downscaledTexture );
		return _downscaledTexture;
	}
}
