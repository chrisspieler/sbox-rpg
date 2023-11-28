using System.Collections.Generic;

namespace Sandbox;

public partial class RpgPlayerController : BaseComponent
{
	public bool EnableAutoWalk { get; set; }
	public bool RunToggle { get; set; }
	[Property] public GameObject Body { get; private set; }
	[Property] public GameObject Eye { get; private set; }
	[Property] public PlayerStateMachine CameraState { get; private set; }
	[Property] public PlayerStateMachine MovementState { get; private set; }
	[Property] public PlayerStateMachine PrimaryHandState { get; private set; }
	[Property] public PlayerStateMachine SecondaryHandState { get; private set; }
	[Property] public CitizenAnimation AnimationHelper { get; set; }
	public Ray EyeRay => new( Eye.Transform.Position, EyeAngles.ToRotation().Forward );
	public CameraComponent PlayerCam
		=> GameObject.GetComponent<CameraComponent>( true, true );
	public CharacterController CharacterController => GameObject.GetComponent<CharacterController>( );
	public Angles EyeAngles;
	// Consider making a separate partial file for state.
	public bool IsFirstPerson => CameraState.CurrentState is FirstPersonCameraState;

	public override void OnStart()
	{
		var startAngle = Transform.Rotation.Forward.EulerAngles
			.WithPitch( 0 )
			.WithRoll( 0 );
		EyeAngles = startAngle;
	}

	public override void Update()
	{
		UpdateBlockers();

		// TODO: Figure out how to stop the player from looking around when
		// rotating a draggable object.
		if ( !IsLookBlocked )
			UpdateEyes();

		HandleInteract();

		if ( Input.Pressed( "view" ) )
			EnableAutoWalk = !EnableAutoWalk;

		if ( Input.Pressed( "menu" ) )
			RunToggle = !RunToggle;
	}

	// TODO: Consider moving this to a separate file.
	public void SetBodyTransparency( float alpha, bool castShadow = true )
	{
		if ( Body is not null )
		{
			// TODO: Cache this, or make callers stop calling it every frame.
			var renderer = Body.GetComponent<AnimatedModelComponent>( false );
			renderer.Tint = renderer.Tint.WithAlpha( alpha );
			renderer.ShouldCastShadows = castShadow;
		}
	}

	private void UpdateEyes()
	{
		EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		EyeAngles.pitch = Math.Clamp( EyeAngles.pitch, -89, 89 );
		EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		EyeAngles.roll = 0;

		// Allows Eye to be treated like aimray.
		Eye.Transform.Rotation = EyeAngles.ToRotation();
	}

	private void HandleInteract()
	{
		EmptyHandState emptyHand = null;

		if ( PrimaryHandState.CurrentState is EmptyHandState primaryEmpty )
			emptyHand = primaryEmpty;
		else if ( SecondaryHandState.CurrentState is EmptyHandState secondaryEmpty )
			emptyHand = secondaryEmpty;

		// No empty hand is free, or no interactable is hovered over.
		if ( emptyHand is null || emptyHand.Hovered is null )
			return;

		var affordances = emptyHand.Hovered.GetComponents<AffordanceComponent>().ToList();

		// If there's nothing we can do with the hovered object, just return.
		if ( !affordances.Any() )
			return;

		foreach( var affordance in affordances )
		{
			if ( Input.Pressed( affordance.ActionButton ) )
				affordance.DoInteract( GameObject, emptyHand );
		}
	}
}
