namespace Sandbox;

public class DraggableComponent : AffordanceComponent
{
	[Property] public PhysicsComponent Rigidbody { get; set; }
	[Property] public float DragSpeed { get; set; } = 50f;
	public override string AffordanceText => "Drag";
	public override string ActionButton => "attack1";
	public DraggingHandState Dragger { get; private set; }
	public override void OnStart()
	{
		Rigidbody ??= GameObject.GetComponent<PhysicsComponent>();
	}

	public override void Update()
	{
		if ( Dragger != null && !Input.Down( ActionButton ) )
			Dragger.ChangeState<EmptyHandState>();
	}
	
	public override void DoInteract( GameObject user, HandState hand )
	{
		Dragger = hand.ChangeState<DraggingHandState>();
		Dragger.Initialize( GameObject, hand.Controller.Eye );
	}
}
