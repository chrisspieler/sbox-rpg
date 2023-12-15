namespace Sandbox;

public class DraggableComponent : AffordanceComponent
{
	[Property] public Rigidbody Rigidbody { get; set; }
	[Property] public float DragSpeed { get; set; } = 50f;
	public override string AffordanceText => "Drag";
	public override string ActionButton => "attack1";

	protected override void OnStart()
	{
		Rigidbody ??= GameObject.Components.Get<Rigidbody>();
	}

	public override void DoInteract( GameObject user, HandState hand )
	{
		var dragger = hand.ChangeState<DraggingHandState>();
		dragger.Initialize( GameObject, hand.Controller.Eye );
	}
}
