namespace Sandbox;

public class RpgPlayerController : BaseComponent
{
	[Property] bool AutoWalkEnabled { get; set; }
	[Property] bool ShouldWalk { get; set; }
	[Property] public Vector3 Gravity { get; set; } = Vector3.Zero.WithZ( 800f );
	[Property] public GameObject Body { get; private set; }
	[Property] public GameObject Eye { get; private set; }
	public Ray EyeRay => new( Eye.Transform.Position, EyeAngles.ToRotation().Forward );
	[Property] public CameraComponent PlayerCam 
		=> GameObject.GetComponent<CameraComponent>( true, true );
	[Property] public PlayerStateMachine CameraState { get; private set; }
	[Property] CitizenAnimation AnimationHelper { get; set; }
	public Angles EyeAngles;
	public Vector3 WishVelocity;

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

		var cc = GameObject.GetComponent<CharacterController>();
		if ( cc is null ) return;

		if ( Input.Pressed( "view" ))
			AutoWalkEnabled = !AutoWalkEnabled;

		if ( Input.Pressed( "menu" ) )
			ShouldWalk = !ShouldWalk;

		float rotateDifference = 0;

		// rotate body to look angles
		if ( Body is not null )
		{
			var targetAngle = new Angles( 0, EyeAngles.yaw, 0 ).ToRotation();

			var v = cc.Velocity.WithZ( 0 );

			if ( v.Length > 10.0f )
			{
				targetAngle = Rotation.LookAt( v, Vector3.Up );
			}

			rotateDifference = Body.Transform.Rotation.Distance( targetAngle );

			var isMovingQuickly = cc.Velocity.Length > 10.0f;
			if ( rotateDifference > 50.0f || isMovingQuickly )
			{
				var rotationSpeedFactor = 2.0f + cc.Velocity.Length / 5.0f;
				Body.Transform.Rotation = Rotation.Lerp( Body.Transform.Rotation, targetAngle, Time.Delta * rotationSpeedFactor );
			}
		}

		if ( AnimationHelper is not null )
		{
			// Prevent Citizen from leaning super far forward while running.
			var velocity = cc.Velocity * (cc.Velocity.Length > 250f ? 0.7f : 1f );
			AnimationHelper.WithVelocity( velocity );
			AnimationHelper.IsGrounded = cc.IsOnGround;
			// I don't see foot shuffling. Perhaps it's not implemented yet?
			AnimationHelper.FootShuffle = rotateDifference;
			var isRunning = Input.Down( "Run" );
			AnimationHelper.WithLook( EyeAngles.Forward, 1, 0.5f, 0.5f );
			AnimationHelper.MoveStyle = isRunning ? CitizenAnimation.MoveStyles.Run : CitizenAnimation.MoveStyles.Walk;
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

	public override void FixedUpdate()
	{
		BuildWishVelocity();

		var cc = GameObject.GetComponent<CharacterController>();

		if ( cc.IsOnGround && Input.Down( "Jump" ) )
		{
			float flGroundFactor = 1.0f;
			float flMul = 268.3281572999747f * 1.2f;

			cc.Punch( Vector3.Up * flMul * flGroundFactor );

			AnimationHelper?.TriggerJump();
		}

		if ( cc.IsOnGround )
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
			cc.Accelerate( WishVelocity );
			cc.ApplyFriction( 4.0f );
		}
		else
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
			cc.Accelerate( WishVelocity.ClampLength( 50 ) );
			cc.ApplyFriction( 0.1f );
		}

		cc.Move();

		if ( !cc.IsOnGround )
		{
			cc.Velocity -= Gravity * Time.Delta * 0.5f;
		}
		else
		{
			cc.Velocity = cc.Velocity.WithZ( 0 );
		}
	}

	public void BuildWishVelocity()
	{
		var rot = EyeAngles.ToRotation();

		WishVelocity = 0;

		if ( Input.Down( "Forward" ) || AutoWalkEnabled ) WishVelocity += rot.Forward;
		if ( Input.Down( "Backward" ) ) WishVelocity += rot.Backward;
		if ( Input.Down( "Left" ) ) WishVelocity += rot.Left;
		if ( Input.Down( "Right" ) ) WishVelocity += rot.Right;

		WishVelocity = WishVelocity.WithZ( 0 );

		if ( !WishVelocity.IsNearZeroLength ) WishVelocity = WishVelocity.Normal;

		if ( Input.Down( "Run" ) ) WishVelocity *= 320.0f;
		else if ( Input.Down( "Walk" ) ) WishVelocity *= 65.0f;
		else WishVelocity *= ShouldWalk ? 65.0f : 110.0f;
	}
}
