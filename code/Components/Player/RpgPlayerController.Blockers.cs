namespace Sandbox;

public partial class RpgPlayerController
{
	public void BlockThirdPerson( BaseComponent blocker ) => _thirdPersonBlockers.Add( blocker );
	public void UnblockThirdPerson( BaseComponent blocker ) => _thirdPersonBlockers.Remove( blocker );
	public bool IsThirdPersonBlocked => _thirdPersonBlockers.Any();
	public void BlockLook( BaseComponent blocker ) => _lookBlockers.Add( blocker );
	public void UnblockLook( BaseComponent blocker ) => _lookBlockers.Remove( blocker );
	public bool IsLookBlocked => _lookBlockers.Any();
	private HashSet<BaseComponent> _thirdPersonBlockers = new HashSet<BaseComponent>();
	private HashSet<BaseComponent> _lookBlockers = new HashSet<BaseComponent>();

	private void UpdateBlockers()
	{
		UpdateBlockerSet( _thirdPersonBlockers );
		UpdateBlockerSet( _lookBlockers );
	}

	private static void UpdateBlockerSet( HashSet<BaseComponent> blockerSource )
	{
		var blockers = blockerSource.ToArray();
		foreach ( var blocker in blockers )
		{
			if ( blocker is null || !blocker.Enabled || blocker.GameObject?.IsValid != true )
			{
				blockerSource.Remove( blocker );
			}
		}
	}
}
