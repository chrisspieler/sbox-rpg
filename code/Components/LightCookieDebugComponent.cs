namespace Sandbox;

public class LightCookieDebugComponent : Component
{
	[Property] public SpotLight SpotLight { get; set; }
	[Property] public bool IgnoreDepth { get; set; } = false;

	protected override void OnUpdate()
	{
		SpotLight ??= Components.Get<SpotLight>();
		if ( SpotLight is null )
			return;

		Gizmo.Draw.IgnoreDepth = IgnoreDepth;
		Gizmo.Draw.Sprite( Transform.Position, new Vector2( 30 ), SpotLight.Cookie, true );
	}
}
