using Sandbox;
using Sandbox.Diagnostics;

public sealed class Mirror : Component, Component.ExecuteInEditor, Component.ITintable
{
	[Property]
	public Model Model
	{
		get => _model;
		set
		{
			if ( _model != value )
			{
				_model = value;
				UpdateObject();
			}
		}
	}
	private Model _model = Model.Load( "models/mirror.vmdl" );

	[Property] public float ClipPlaneOffset { get; set; } = 1.0f;
	[Property] public bool DrawDebug { get; set; } = false;

	private Color _tint = Color.White;
	private bool _castShadows = true;
	private SceneObject _sceneObject;
	private SceneCamera _sceneCamera;
	private Texture _mirrorTexture;

	public BBox Bounds
	{
		get
		{
			if ( _sceneObject != null )
			{
				return _sceneObject.Bounds;
			}

			return BBox.FromPositionAndSize( Transform.Position, 16f );
		}
	}

	[Property]
	public Color Tint
	{
		get
		{
			return _tint;
		}
		set
		{
			if ( !(_tint == value) )
			{
				_tint = value;
				UpdateObject();
			}
		}
	}

	[Property]
	public bool ShouldCastShadows
	{
		get
		{
			return _castShadows;
		}
		set
		{
			if ( _castShadows != value )
			{
				_castShadows = value;
				UpdateObject();
			}
		}
	}

	public SceneObject SceneObject => _sceneObject;

	Color ITintable.Color
	{
		get
		{
			return Tint;
		}
		set
		{
			Tint = value;
		}
	}

	protected override void DrawGizmos()
	{
		if ( Model is null )
			return;

		Gizmo.Hitbox.Model( Model );
		Gizmo.Draw.Color = Color.White.WithAlpha( 0.1f );
		if ( Gizmo.IsSelected )
		{
			Gizmo.Draw.Color = Color.White.WithAlpha( 0.9f );
			Gizmo.GizmoDraw draw = Gizmo.Draw;
			BBox box = Model.Bounds;
			draw.LineBBox( in box );
		}

		if ( Gizmo.IsHovered )
		{
			Gizmo.Draw.Color = Color.White.WithAlpha( 0.4f );
			Gizmo.GizmoDraw draw2 = Gizmo.Draw;
			BBox box = Model.Bounds;
			draw2.LineBBox( in box );
		}
	}

	private void UpdateObject()
	{
		if ( !_sceneObject.IsValid() )
			return;

		_sceneObject.ColorTint = Tint;
		_sceneObject.Flags.CastShadows = _castShadows;
		_sceneObject.Model = Model;

		_sceneCamera = new SceneCamera()
		{
			World = Scene.SceneWorld
		};
		_sceneCamera.ExcludeTags.Add( "firstperson" );
		_sceneObject.RenderingEnabled = false;
	}

	protected override void OnEnabled()
	{
		Assert.True( _sceneObject == null );
		Assert.NotNull( Scene );
		_sceneObject = new SceneObject( Scene.SceneWorld, Model, Transform.World );
		_sceneObject.Tags.SetFrom( GameObject.Tags );
		_mirrorTexture = Texture.CreateRenderTarget( "mirror", ImageFormat.Default, new Vector2( 512 ) );
		_sceneObject.Attributes.Set( "Color", _mirrorTexture );
		UpdateObject();
	}

	protected override void OnDisabled()
	{
		_sceneObject?.Delete();
		_sceneObject = null;
		_sceneCamera?.Dispose();
		_sceneCamera = null;
	}

	protected override void OnPreRender()
	{
		if ( !_sceneObject.IsValid() )
			return;

		_sceneObject.Transform = Transform.World;

		UpdateScenePortal();
	}

	protected override void OnUpdate()
	{
		if ( DrawDebug )
			DrawDebugInfo();
	}

	private void DrawDebugInfo()
	{

		var startPos = Transform.Position + Transform.Rotation.Up * 100f;
		Gizmo.Draw.Sprite( startPos, 80f, _mirrorTexture );

		if ( _sceneCamera is null )
			return;

		Gizmo.Draw.Color = Color.Red;
		Gizmo.Draw.IgnoreDepth = true;
		Gizmo.Draw.LineSphere( new Sphere( _sceneCamera.Position, 5f ) );
	}

	private void UpdateScenePortal()
	{
		Plane p = new( Transform.Position, Transform.Rotation.Up );
		var cameraTx = Scene.Camera.Transform;
		var viewMatrix = Matrix.CreateWorld( cameraTx.Position, cameraTx.Rotation.Forward, cameraTx.Rotation.Up );
		var reflectMatrix = ReflectMatrix( viewMatrix, p );

		var aspect = Screen.Width / Screen.Height;
		var fov = MathF.Atan( MathF.Tan( Scene.Camera.FieldOfView.DegreeToRadian() * 0.41f ) * ( aspect * 0.75f ) ).RadianToDegree() * 2.0f;

		if ( _sceneCamera is null ) return;

		_sceneCamera.Position = reflectMatrix.Transform( cameraTx.Position );
		_sceneCamera.Rotation = ReflectRotation( cameraTx.Rotation, Transform.Rotation.Up );
		_sceneCamera.FieldOfView = fov;
		_sceneCamera.Attributes.Set("EnableClipPlane", true );
		var clipPlane = new Plane( Transform.Position - _sceneCamera.Position, Transform.Rotation.Up );
		clipPlane.Distance -= ClipPlaneOffset;
		_sceneCamera.Attributes.Set( "ClipPlane0", new Vector4( clipPlane.Normal, clipPlane.Distance ) );

		Graphics.RenderToTexture( _sceneCamera, _mirrorTexture );
	}

	protected override void OnTagsChanged()
	{
		if ( _sceneObject.IsValid() )
		{
			_sceneObject.Tags.SetFrom( GameObject.Tags );
		}
	}

	public Matrix ReflectMatrix( System.Numerics.Matrix4x4 m, Plane plane )
	{
		m.M11 = (1.0f - 2.0f * plane.Normal.x * plane.Normal.x);
		m.M21 = (-2.0f * plane.Normal.x * plane.Normal.y);
		m.M31 = (-2.0f * plane.Normal.x * plane.Normal.z);
		m.M41 = (-2.0f * -plane.Distance * plane.Normal.x);

		m.M12 = (-2.0f * plane.Normal.y * plane.Normal.x);
		m.M22 = (1.0f - 2.0f * plane.Normal.y * plane.Normal.y);
		m.M32 = (-2.0f * plane.Normal.y * plane.Normal.z);
		m.M42 = (-2.0f * -plane.Distance * plane.Normal.y);

		m.M13 = (-2.0f * plane.Normal.z * plane.Normal.x);
		m.M23 = (-2.0f * plane.Normal.z * plane.Normal.y);
		m.M33 = (1.0f - 2.0f * plane.Normal.z * plane.Normal.z);
		m.M43 = (-2.0f * -plane.Distance * plane.Normal.z);

		m.M14 = 0.0f;
		m.M24 = 0.0f;
		m.M34 = 0.0f;
		m.M44 = 1.0f;

		return m;
	}

	private Rotation ReflectRotation( Rotation source, Vector3 normal )
	{
		return Rotation.LookAt( Vector3.Reflect( source * Vector3.Forward, normal ), Vector3.Reflect( source * Vector3.Up, normal ) );
	}
}
