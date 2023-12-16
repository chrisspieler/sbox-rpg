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
	private Model _model = Model.Load( "models/dev/plane.vmdl" );

	[Property] public float ClipPlaneOffset { get; set; } = 1.0f;

	public static readonly Material MirrorMaterial = Material.Load( "materials/mirror.vmat" );

	private Color _tint = Color.White;
	private bool _castShadows = true;
	private SceneObject _sceneObject;
	private ScenePortal _scenePortal;

	public BBox Bounds
	{
		get
		{
			if ( _sceneObject != null )
			{
				return _sceneObject.Bounds;
			}

			return new BBox( base.Transform.Position, 16f );
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
		_sceneObject.SetMaterialOverride( MirrorMaterial );

		_scenePortal?.Delete();
		_scenePortal = new ScenePortal( Scene.SceneWorld, Model, Transform.World, true, (int)Screen.Width )
		{
			RenderShadows = true,
			RenderingEnabled = true
		};
		_scenePortal.SetMaterialOverride( MirrorMaterial );
		_sceneObject.RenderingEnabled = false;
	}

	protected override void OnEnabled()
	{
		Assert.True( _sceneObject == null );
		Assert.NotNull( Scene );
		_sceneObject = new SceneObject( Scene.SceneWorld, Model, Transform.World );
		_sceneObject.Tags.SetFrom( GameObject.Tags );
		UpdateObject();
	}

	protected override void OnDisabled()
	{
		_sceneObject?.Delete();
		_sceneObject = null;
		_scenePortal?.Delete();
		_scenePortal = null;
	}

	protected override void OnPreRender()
	{
		if ( !_sceneObject.IsValid() )
			return;

		_sceneObject.Transform = Transform.World;

		if ( !_scenePortal.IsValid() )
			return;

		_scenePortal.RenderingEnabled = true;
		_scenePortal.Transform = Transform.World;
		Plane p = new( Transform.Position, Transform.Rotation.Forward );
		var viewMatrix = Matrix.CreateWorld( Camera.Position, Camera.Rotation.Forward, Camera.Rotation.Up );
		var reflectMatrix = ReflectMatrix( viewMatrix, p );

		_scenePortal.ViewPosition = reflectMatrix.Transform( Camera.Position ); ;
		_scenePortal.ViewRotation = ReflectRotation( Camera.Rotation, Transform.Rotation.Forward );
		_scenePortal.Aspect = Screen.Width / Screen.Height;
		_scenePortal.FieldOfView = Camera.FieldOfView;

		var clipPlane = new Plane( Transform.Position - _scenePortal.ViewPosition, Transform.Rotation.Forward );
		clipPlane.Distance -= ClipPlaneOffset;

		_scenePortal.SetClipPlane( clipPlane );
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
