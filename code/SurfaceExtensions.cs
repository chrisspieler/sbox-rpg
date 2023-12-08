namespace Sandbox;

public static class SurfaceExtensions
{
	public static void DoFootstep(this Surface surf, PhysicsTraceResult tr, int foot, float volume)
	{
		var sound = foot == 0 ? surf.Sounds.FootLeft : surf.Sounds.FootRight;

		if ( !string.IsNullOrWhiteSpace( sound ) )
		{
			var hSnd = Sound.Play( sound, tr.EndPosition );
			hSnd.Volume = volume;
		}
		else
		{
			// Give base surface a chance
			surf.GetBaseSurface().DoFootstep( tr, foot, volume );
		}
	}
}
