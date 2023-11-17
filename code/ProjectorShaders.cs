namespace Sandbox;

public static class ProjectorShaders
{
	private static readonly ComputeShader _csDownscale = new ComputeShader( "downscale_cs" );
	private static readonly ComputeShader _csMultiply = new ComputeShader( "multiplytexture_cs" );
	private static readonly ComputeShader _csBlur = new ComputeShader( "gaussianblur_cs" );
	private static readonly ComputeShader _csColorPad = new ComputeShader( "colorpad_cs" );

	public static void DispatchDownscale( Texture inTex, Texture outTex )
	{
		_csDownscale.Attributes.Set( "InputTexture", inTex );
		_csDownscale.Attributes.Set( "OutputTexture", outTex );
		_csDownscale.Dispatch( outTex.Width, outTex.Height, 1 );
	}

	public static void DispatchMultiply( Texture inTex, Texture multTex, Texture outTex )
	{
		_csMultiply.Attributes.Set( "InputTexture", inTex );
		_csMultiply.Attributes.Set( "MultiplicandTexture", multTex );
		_csMultiply.Attributes.Set( "OutputTexture", outTex );
		_csMultiply.Dispatch( outTex.Width, outTex.Height, 1 );
	}

	public static void DispatchGaussianBlur( Texture inTex, Texture outTex )
	{
		_csBlur.Attributes.Set( "InputTexture", inTex );
		_csBlur.Attributes.Set( "OutputTexture", outTex );
		_csBlur.Dispatch( outTex.Width, outTex.Height, 1 );
	}

	public static void DispatchColorPad( Texture inTex, Texture outTex, Color padColor,
		float padRatio, float fitAspectRatio )
	{
		_csColorPad.Attributes.Set( "InputTexture", inTex );
		_csColorPad.Attributes.Set( "OutputTexture", outTex );
		_csColorPad.Attributes.Set( "PadColor", padColor );
		_csColorPad.Attributes.Set( "PadRatio", padRatio );
		_csColorPad.Attributes.Set( "FitAspectRatio", fitAspectRatio );
		_csColorPad.Dispatch( outTex.Width, outTex.Height, 1 );
	}
}
