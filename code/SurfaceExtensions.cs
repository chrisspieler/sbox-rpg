namespace Sandbox;

public static class SurfaceExtensions
{
	public static void DoFootstep(this Surface surf, PhysicsTraceResult tr, int foot, float volume)
	{
		var sound = foot == 0 ? surf.Sounds.FootLeft : surf.Sounds.FootRight;

		if ( !string.IsNullOrWhiteSpace( sound ) )
		{
			Sound.FromWorld( sound, tr.EndPosition ).SetVolume( volume );
		}
		else
		{
			// Give base surface a chance
			surf.GetBaseSurface().DoFootstep( tr, foot, volume );
		}
	}
}
