namespace Sandbox;

public sealed class MonitorComponent : DynamicTextureComponent
{
	[Property] public ModelRenderer Model { get; set; }
	[Property] public string AttributeName { get; set; } = "screen";
	public override Texture OutputTexture
	{
		get => base.OutputTexture;
		protected set
		{
			base.OutputTexture = value;
			ApplyMaterialOverride();
		}
	}

	private Material _screenMaterial;

	protected override void OnStart()
	{
		Model ??= GameObject.Components.Get<ModelRenderer>();
	}

	public override void OnPostEffect()
	{
		if ( _screenMaterial is null )
		{
			ApplyMaterialOverride();
		}
	}

	public void ApplyMaterialOverride()
	{
		if ( !GameManager.IsPlaying || Model?.SceneObject is null )
			return;

		_screenMaterial = Material.Create( "ScreenMaterial", "generic" );
		_screenMaterial.Set( "Color", OutputTexture );

		if ( string.IsNullOrWhiteSpace( AttributeName ) )
		{
			Model.MaterialOverride = _screenMaterial;
		}
		else
		{
			Model.SceneObject.SetMaterialOverride( _screenMaterial , AttributeName );
		}
	}
}
