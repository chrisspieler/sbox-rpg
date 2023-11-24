﻿namespace Sandbox;

public class DraggableComponent : AffordanceComponent
{
	[Property] public PhysicsComponent Rigidbody { get; set; }
	[Property] public float DragSpeed { get; set; } = 50f;
	public override string AffordanceText => "Drag";
	public override string ActionButton => "attack1";

	public override void OnStart()
	{
		Rigidbody ??= GameObject.GetComponent<PhysicsComponent>();
	}

	public override void DoInteract( GameObject user, HandState hand )
	{
		var dragger = hand.ChangeState<DraggingHandState>();
		dragger.Initialize( GameObject, hand.Controller.Eye );
	}
}
