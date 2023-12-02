namespace Sandbox;

public partial class Npc
{
	public GameObject HeldObject { get; private set; }
	public event EventHandler<GameObject> HeldObjectChanged;
	private bool _holdEnabled { get; set; } = false;

	public void SetHoldEnabled( bool holdEnabled )
	{
		var collector = Components.Get<TriggerCollectorComponent>( FindMode.EverythingInSelfAndChildren );
		collector.GameObject.Enabled = holdEnabled;
		var snapPoint = Components.Get<TriggerSnapPointComponent>( FindMode.EverythingInSelfAndChildren );
		if ( holdEnabled )
		{
			snapPoint.SnappedChanged += SnappedChanged;
		}
		else
		{
			snapPoint.SnappedChanged -= SnappedChanged;
		}
	}

	private void SnappedChanged( object sender, GameObject snapped )
	{
		HeldObject = snapped;
		HeldObjectChanged?.Invoke( this, HeldObject );
	}
}
