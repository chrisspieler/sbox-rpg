using Sandbox.Citizen;

namespace Sandbox;

public partial class RpgPlayerController : Component
{
	public static RpgPlayerController Instance { get; private set; }

	public bool EnableAutoWalk { get; set; }
	public bool RunToggle { get; set; }
	[Property] public GameObject Body { get; private set; }
	[Property] public SkinnedModelRenderer BodyModel { get; private set; }
	[Property] public GameObject Eye { get; private set; }
	[Property] public PlayerStateMachine CameraState { get; private set; }
	[Property] public PlayerStateMachine MovementState { get; private set; }
	[Property] public PlayerStateMachine PrimaryHandState { get; private set; }
	[Property] public PlayerStateMachine SecondaryHandState { get; private set; }
	[Property] public CitizenAnimationHelper AnimationHelper { get; set; }
	public Ray EyeRay => new( Eye.Transform.Position, EyeAngles.ToRotation().Forward );
	public CameraComponent PlayerCam
		=> GameObject.Components.Get<CameraComponent>( FindMode.EverythingInSelfAndDescendants );
	public CharacterController CharacterController => GameObject.Components.Get<CharacterController>( );
	public Angles EyeAngles;
	// Consider making a separate partial file for state.
	public bool IsFirstPerson => CameraState.CurrentState is FirstPersonCameraState;

	public RpgPlayerController()
	{
		Instance = this;
	}

	protected override void OnStart()
	{
		var startAngle = Transform.Rotation.Forward.EulerAngles
			.WithPitch( 0 )
			.WithRoll( 0 );
		EyeAngles = startAngle;
	}

	protected override void OnUpdate()
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
		if ( BodyModel is not null )
		{
			BodyModel.Tint = BodyModel.Tint.WithAlpha( alpha );
			BodyModel.RenderType = castShadow ? ModelRenderer.ShadowRenderType.On : ModelRenderer.ShadowRenderType.Off;
			var outfit = Components.Get<Outfit>( true );
			if ( outfit is not null )
			{
				// Quick and dirty fix for body group issues when switching out of first person.
				outfit.Enabled = alpha > 0;
			}
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

		var affordances = emptyHand.GetAffordancesFromHovered();

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
