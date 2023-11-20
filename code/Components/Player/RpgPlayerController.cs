namespace Sandbox;

public class RpgPlayerController : BaseComponent
{
	public bool EnableAutoWalk { get; set; }
	public bool RunToggle { get; set; }
	[Property] public GameObject Body { get; private set; }
	[Property] public GameObject Eye { get; private set; }
	[Property] public PlayerStateMachine CameraState { get; private set; }
	[Property] public PlayerStateMachine MovementState { get; private set; }
	[Property] public CitizenAnimation AnimationHelper { get; set; }
	public Ray EyeRay => new( Eye.Transform.Position, EyeAngles.ToRotation().Forward );
	public CameraComponent PlayerCam
		=> GameObject.GetComponent<CameraComponent>( true, true );
	public CharacterController CharacterController => GameObject.GetComponent<CharacterController>( );
	public Angles EyeAngles;

	public override void OnStart()
	{
		var startAngle = Transform.Rotation.Forward.EulerAngles
			.WithPitch( 0 )
			.WithRoll( 0 );
		EyeAngles = startAngle;
	}

	public override void Update()
	{
		// TODO: Figure out how to stop the player from looking around when
		// rotating a draggable object.
		UpdateEyes();

		HandleInteract();

		if ( Input.Pressed( "view" ) )
			EnableAutoWalk = !EnableAutoWalk;

		if ( Input.Pressed( "menu" ) )
			RunToggle = !RunToggle;

		var cc = GameObject.GetComponent<CharacterController>();
		if ( cc is null ) return;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			var v = cc.Velocity.WithZ( 0 );

			if ( v.Length > 10.0f )
			{
				targetAngle = Rotation.LookAt( v, Vector3.Up );
			}

			var rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			var isMovingQuickly = cc.Velocity.Length > 10.0f;
			if ( rotateDifference > 50.0f || isMovingQuickly )
			{
				var rotationSpeedFactor = 2.0f + cc.Velocity.Length / 5.0f;
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * rotationSpeedFactor );
			}
		}
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
		var interact = GameObject.GetComponent<InteractableTraceComponent>();

		// The player may not interact, or no interactable is hovered over.
		if ( interact is null || interact.Hovered is null )
			return;

		var affordances = interact.Hovered.GetComponents<AffordanceComponent>();

		// If there's nothing we can do with the hovered object, just return.
		if ( !affordances.Any() )
			return;

		foreach( var affordance in affordances )
		{
			// TODO: Figure out how to to pass the Eye to the Draggable affordance.
			if ( Input.Pressed( affordance.ActionButton ) )
				affordance.DoInteract( GameObject );
		}
	}
}
