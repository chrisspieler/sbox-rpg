namespace Sandbox;

public class LightSwitchComponent : BaseComponent
{
	[Property] public Color OnColor { get; set; } = Color.White;
	[Property] public string OnMaterialGroup { get; set; }
	[Property] public Color OffColor { get; set; } = Color.Black;
	[Property] public string OffMaterialGroup { get; set; }
	[Property] public PointLightComponent TargetLight { get; set; }
	[Property] public ModelComponent TargetLightModel { get; set; }

	[Property]
	public InteractableComponent Interactable
	{
		get => _interactable;
		set
		{
			if ( !global::GameManager.IsPlaying )
			{
				_interactable = value;
				return;
			}

			if ( _interactable is not null )
				_interactable.OnInteract -= InteractableOnInteract;

			_interactable = value;
			_interactable.OnInteract += InteractableOnInteract;
		}
	}
	private InteractableComponent _interactable;
	[Property] public bool IsOn
	{
		get => _isOn;
		set
		{
			if ( !global::GameManager.IsPlaying )
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

	public override void OnStart()
	{
		base.OnStart();

		if ( IsOn )
		{
			TurnOn();
		}
		else
		{
			TurnOff();
		}
	}

	private void InteractableOnInteract( object sender, GameObject e )
	{
		ToggleLight();
	}

	public void ToggleLight() => IsOn = !IsOn;
	public void TurnOn()
	{
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
