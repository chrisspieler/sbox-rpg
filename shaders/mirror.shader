//=========================================================================================================================
// Optional
//=========================================================================================================================
HEADER
{
	CompileTargets = ( IS_SM_50 && ( PC || VULKAN ) );
	Description = "portalz";
}

//=========================================================================================================================
// Optional
//=========================================================================================================================
FEATURES
{
    #include "common/features.hlsl"
}

//=========================================================================================================================
COMMON
{
	#include "common/shared.hlsl"
}

//=========================================================================================================================

struct VertexInput
{
	#include "common/vertexinput.hlsl"
};

//=========================================================================================================================

struct PixelInput
{
	#include "common/pixelinput.hlsl"
};

//=========================================================================================================================

VS
{
	#include "common/vertex.hlsl"
	//
	// Main
	//
	PixelInput MainVs( VS_INPUT i )
	{
		PixelInput o = ProcessVertex( i );
		// Add your vertex manipulation functions here

		return FinalizeVertex( o );
	}
}

//=========================================================================================================================

PS
{
	#include "sbox_pixel.fxc"

	CreateInputTexture2D( Texture, Srgb, 8, "", "", "Color", Default3( 1.0, 1.0, 1.0 ) );
	CreateTexture2DInRegister( g_tColor, 0 ) < Channel( RGBA, None( Texture ), Srgb ); OutputFormat( RGBA16161616F ); SrgbRead( true ); >;


	float4 MainPs( PixelInput i ) : SV_Target0
	{
		float4 o;
		float2 psuv = i.vPositionSs.xy;
		float2 vScreenUv = CalculateViewportUv( psuv ); // = ScreenspaceCorrectionMultiview( CalculateViewportUv( psuv) );

		// mirror
		vScreenUv.x = 1 - vScreenUv.x;


        o = Tex2D( g_tColor, vScreenUv );

		return o;
	}
}