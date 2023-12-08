namespace Sandbox;

public class ProjectorComponent : DynamicTextureComponent
{
	[Property] public SpotLight Light { get; set; }

	public override void OnPostEffect()
	{
		if ( Light.Cookie != OutputTexture )
		{
			Light.Cookie = OutputTexture;
		}
	}
}
