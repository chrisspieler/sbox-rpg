namespace Sandbox;

public partial class RpgPlayerController
{
	public void BlockThirdPerson( Component blocker ) => _thirdPersonBlockers.Add( blocker );
	public void UnblockThirdPerson( Component blocker ) => _thirdPersonBlockers.Remove( blocker );
	public bool IsThirdPersonBlocked => _thirdPersonBlockers.Any();
	public void BlockLook( Component blocker ) => _lookBlockers.Add( blocker );
	public void UnblockLook( Component blocker ) => _lookBlockers.Remove( blocker );
	public bool IsLookBlocked => _lookBlockers.Any();
	public void BlockMovement( Component blocker ) => _movementBlockers.Add( blocker );
	public void UnblockMovement( Component blocker ) => _movementBlockers.Remove( blocker );
	public bool IsMovementBlocked => _movementBlockers.Any();

	private HashSet<Component> _thirdPersonBlockers = new HashSet<Component>();
	private HashSet<Component> _lookBlockers = new HashSet<Component>();
	private HashSet<Component> _movementBlockers = new HashSet<Component>();

	private void UpdateBlockers()
	{
		UpdateBlockerSet( _thirdPersonBlockers );
		UpdateBlockerSet( _lookBlockers );
	}

	private static void UpdateBlockerSet( HashSet<Component> blockerSource )
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
