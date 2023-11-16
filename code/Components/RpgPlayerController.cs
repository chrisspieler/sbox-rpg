using System;
using System.Collections.Generic;
namespace Sandbox;

public class RpgPlayerController : BaseComponent
{
	[Property] bool AutoWalkEnabled { get; set; }
	[Property] bool ShouldWalk { get; set; }
	/// <summary>
	/// If true, the camera will be positioned at <see cref="Eye"/>. Otherwise, it will
	/// orbit around <see cref="Eye"/> at a distance of <see cref="CameraDistance"/>.
	/// </summary>
	[Property] bool FirstPerson { get; set; }
	[Property, Range( 20f, 150f )] float FirstPersonFov { get; set; } = 100.0f;
	[Property, Range( 20f, 150f )] float ThirdPersonFov { get; set; } = 60.0f;
	[Property, Range( 50, 400 )] public float CameraDistance { get; set; } = 200.0f;
	[Property] public Vector3 ThirdPersonCameraOffset { get; set; } = Vector3.Zero.WithZ( -20f );
	[Property] public Vector3 Gravity { get; set; } = Vector3.Zero.WithZ( 800f );
	[Property] GameObject Body { get; set; }
	[Property] GameObject Eye { get; set; }
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
		EyeAngles.pitch += Input.MouseDelta.y * 0.1f;
		EyeAngles.pitch = Math.Clamp( EyeAngles.pitch, -89, 89 );
		EyeAngles.yaw -= Input.MouseDelta.x * 0.1f;
		EyeAngles.roll = 0;

		var camera = GameObject.GetComponent<CameraComponent>( true, true );
		if ( camera is not null )
		{
			if ( Input.Pressed( "flashlight" ) )
				FirstPerson = !FirstPerson;

			camera.FieldOfView = FirstPerson ? FirstPersonFov : ThirdPersonFov;

			if ( Body is not null )
			{
				var renderer = Body.GetComponent<AnimatedModelComponent>( false );
				renderer.Enabled = !FirstPerson;
			}

			var mouseInput = Input.MouseWheel * 30.0f;
			if ( FirstPerson )
			{
				if ( mouseInput < 0 )
				{
					// We are zooming out from first person.
					FirstPerson = false;
					CameraDistance = 50f;
				}
			}
			else
			{
				CameraDistance -= mouseInput;
				CameraDistance = CameraDistance.Clamp( 49f, 400f );
				if ( CameraDistance < 50f )
					FirstPerson = true;
			}

			var camPos = Eye.Transform.Position - EyeAngles.ToRotation().Forward * CameraDistance;

			if ( FirstPerson )
			{
				camPos = Eye.Transform.Position;
			}
			else
			{
				camPos += ThirdPersonCameraOffset;
				var tr = Scene.PhysicsWorld
					.Trace
					.Ray( Eye.Transform.Position, camPos )
					.Run();
				if ( tr.Hit )
				{
					camPos = tr.HitPosition;
				}
			}

			camera.Transform.Position = camPos;
			camera.Transform.Rotation = EyeAngles.ToRotation();
		}

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
		else if ( Input.Down( "Walk" ) ) WishVelocity *= 70.0f;
		else WishVelocity *= ShouldWalk ? 70.0f : 140.0f;
	}
}
