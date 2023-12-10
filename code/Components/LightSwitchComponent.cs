namespace Sandbox;

public class LightSwitchComponent : AffordanceComponent
{
	[Property] public Color OnColor { get; set; } = Color.White;
	[Property] public string OnMaterialGroup { get; set; }
	[Property] public Color OffColor { get; set; } = Color.Black;
	[Property] public string OffMaterialGroup { get; set; }
	[Property] public PointLight TargetLight { get; set; }
	[Property] public ModelRenderer TargetLightModel { get; set; }
	public override string AffordanceText => "Toggle Light";

	[Property] public bool IsOn
	{
		get => _isOn;
		set
		{
			if ( !GameManager.IsPlaying )
			{
				_isOn = value;
				return;
			}

			if ( value )
			{
				TurnOn();
			}
			else
			{
				TurnOff();
			}
		}
	}
	private bool _isOn;

	protected override void OnStart()
	{
		if ( IsOn )
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}

	public override void DoInteract( GameObject user, HandState hand )
	{
		ToggleLight();
	}

	public void ToggleLight() => IsOn = !IsOn;
	public void TurnOn()
	{
		if ( TargetLight is null )
			return;

		TargetLight.Enabled = true;
		TargetLight.LightColor = OnColor;

		if ( !string.IsNullOrWhiteSpace( OnMaterialGroup ) && TargetLightModel is not null )
		{
			TargetLightModel.SceneObject.SetMaterialGroup( OnMaterialGroup );
		}
		_isOn = true;
	}

	public void TurnOff()
	{
		if ( TargetLight is null )
			return;

		TargetLight.LightColor = OffColor;
		if ( OffColor == Color.Black )
		{
			TargetLight.Enabled = false;
		}

		if ( !string.IsNullOrWhiteSpace( OffMaterialGroup ) && TargetLightModel is not null )
		{
			TargetLightModel.SceneObject.SetMaterialGroup( OffMaterialGroup );
		}
		_isOn = false;
	}


}
