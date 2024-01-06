namespace Sandbox;

public sealed class MonitorComponent : DynamicTextureComponent
{
	[Property] public ModelRenderer Model { get; set; }
	[Property] public string AttributeName { get; set; } = "screen";
	[Property] public string MaterialName { get; set; }

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

		if ( !string.IsNullOrWhiteSpace( MaterialName ) )
		{
			_screenMaterial = Model.Model.Materials.FirstOrDefault( m => m.Name == MaterialName );
			if ( _screenMaterial is null )
			{
				return;
			}
			_screenMaterial.Set( "Color", OutputTexture );
			return;
		}

		_screenMaterial = Material.Load( "materials/lcd_screen.vmat" ).CreateCopy();
		_screenMaterial.Set( "Color", OutputTexture );
		_screenMaterial.Set( "SelfIllumMask", OutputTexture );

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
