namespace Sandbox;

public class TriggerSnapPointComponent : BaseComponent
{
	[Property] public TriggerCollectorComponent CollectorTarget { get; set; }
	[Property] public GameObject Snapped 
	{
		get => _snapped;
		set
		{
			var oldValue = _snapped;
			_snapped = value;
			if ( _snapped != oldValue )
			{
				SnappedChanged?.Invoke( this, _snapped );
			}
		}
	}
	private GameObject _snapped;
	public event EventHandler<GameObject> SnappedChanged;

	protected override void OnStart()
	{
		CollectorTarget.TriggerEnter += CollectorTriggerEnter;
	}

	protected override void OnUpdate()
	{
		if ( Snapped?.IsValid != true )
			return;

		Snapped.Transform.LocalPosition = Vector3.Zero;
		Snapped.Transform.LocalRotation = Rotation.Identity;
	}

	private void CollectorTriggerEnter( object sender, Collider collider )
	{
		if ( Snapped?.IsValid == true )
			return;

		collider.GameObject.Parent = GameObject;
		// If this object is being dragged, this will terminate the drag.
		collider.GameObject.Tags.Remove( "held" );
		collider.Components.Get<DraggableComponent>()?.SetEnabled( false );
		collider.Components.Get<PhysicsComponent>()?.SetEnabled( false );
		collider.Transform.LocalPosition = Vector3.Zero;
		collider.Transform.LocalRotation = Rotation.Identity;
		Snapped = collider.GameObject;
		SnappedChanged?.Invoke( this, Snapped );
	}
}
