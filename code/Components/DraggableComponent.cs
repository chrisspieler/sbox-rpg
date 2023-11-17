namespace Sandbox;

public class DraggableComponent : AffordanceComponent
{
	[Property] public PhysicsComponent Rigidbody { get; set; }
	[Property] public float DragSpeed { get; set; } = 50f;
	[Property] public override string AffordanceText => "Drag";
	[Property] public override string ActionButton => "attack1";
	public GameObject Dragger { get; private set; }
	public float MaxHoldDistance = 80f;
	public float DefaultHoldDistance = 60f;
	public float MinHoldDistance = 30f;

	private float _currentHoldDistance;

	public override void OnStart()
	{
		Rigidbody ??= GameObject.GetComponent<PhysicsComponent>();
	}

	public override void FixedUpdate()
	{
		if ( Dragger is null )
			return;

		var dragTo = Dragger.Transform.Position + Dragger.Transform.Rotation.Forward * _currentHoldDistance;
		var direction = ( dragTo - Rigidbody.Transform.Position ).Normal;
		var distance = dragTo.Distance( Rigidbody.Transform.Position );
		var scaledSpeed = MathX.Lerp( 10f, DragSpeed * 5, distance / DefaultHoldDistance );
		Rigidbody.Velocity = direction * scaledSpeed;
		// Gradually stop the rotation.
		Rigidbody.AngularVelocity = Rigidbody.AngularVelocity.LerpTo( Vector3.Zero, 2.5f * Time.Delta );

		if ( Input.Down( "attack2" ) )
		{
			Rigidbody.AngularVelocity = Vector3.Zero;

			// Yoinked rotation code from the Sandbox physgun.
			var eyeRot = Rotation.From( new Angles( 0f, Camera.Main.Rotation.Yaw(), 0f ) );
			var localRot = eyeRot;
			localRot *= Rotation.FromAxis( Vector3.Up, Input.MouseDelta.x * 0.3f );
			localRot *= Rotation.FromAxis( Vector3.Right, Input.MouseDelta.y * 0.3f );
			localRot = eyeRot.Inverse * localRot;
			Transform.Rotation = localRot * Transform.Rotation;
		}

		var mouseInput = Input.MouseWheel * 30.0f;
		if ( mouseInput != 0 )
		{
			_currentHoldDistance += mouseInput;
			_currentHoldDistance = _currentHoldDistance.Clamp( MinHoldDistance, MaxHoldDistance );
		}
	}

	public void BeginDrag( GameObject go )
	{
		Dragger = go;
		_currentHoldDistance = DefaultHoldDistance;
	}

	public void EndDrag()
	{
		Dragger = null;
	}

	public override void DoInteract( GameObject user )
	{
		throw new NotImplementedException();
	}
}
