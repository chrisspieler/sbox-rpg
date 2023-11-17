namespace Sandbox;

public class LightCookieDebugComponent : BaseComponent
{
	[Property] public SpotLightComponent SpotLight { get; set; }
	[Property] public bool IgnoreDepth { get; set; } = false;

	public override void Update()
	{
		base.Update();

		SpotLight ??= GetComponent<SpotLightComponent>();
		if ( SpotLight is null )
			return;

		Gizmo.Draw.IgnoreDepth = IgnoreDepth;
		Gizmo.Draw.Sprite( Transform.Position, new Vector2( 30 ), SpotLight.Cookie, true );
	}
}
