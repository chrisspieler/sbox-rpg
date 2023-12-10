namespace Sandbox;

public class Human : Component, Component.ExecuteInEditor
{
	[Property] public SkinnedModelRenderer Body { get; set; }
	[Property] public GenderType Gender { get; set; }
	[Property] public SkinToneType SkinTone
	{
		get => _skinTone;
		set
		{
			_skinTone = value;
			UpdateSkin();
		}
	}
	private SkinToneType _skinTone;
	[Property] public AgeType Age
	{
		get => _age;
		set
		{
			_age = value;
			UpdateSkin();
		}
	}
	private AgeType _age;

	public Material SkinMaterial => Material.Load( _averageBuildBodyMaterials[(_age, _skinTone)] );

	private static Dictionary<(AgeType, SkinToneType), string> _averageBuildBodyMaterials = new();
	static Human()
	{
		_averageBuildBodyMaterials.Clear();
		_averageBuildBodyMaterials[(AgeType.Adult, SkinToneType.Pale)] = "models/citizen/skin/citizen_skin04.vmat";
		_averageBuildBodyMaterials[(AgeType.Adult, SkinToneType.Light)] = "models/citizen/skin/citizen_skin01.vmat";
		_averageBuildBodyMaterials[(AgeType.Adult, SkinToneType.Medium)] = "models/citizen/skin/citizen_skin02.vmat";
		_averageBuildBodyMaterials[(AgeType.Adult, SkinToneType.Dark)] = "models/citizen/skin/citizen_skin03.vmat";
		_averageBuildBodyMaterials[(AgeType.Elder, SkinToneType.Pale)] = "models/citizen/skin/citizen_skin08.vmat";
		_averageBuildBodyMaterials[(AgeType.Elder, SkinToneType.Light)] = "models/citizen/skin/citizen_skin06.vmat";
		_averageBuildBodyMaterials[(AgeType.Elder, SkinToneType.Medium)] = "models/citizen/skin/citizen_skin07.vmat";
		_averageBuildBodyMaterials[(AgeType.Elder, SkinToneType.Dark)] = "models/citizen/skin/citizen_skin05.vmat";
	}

	protected override void OnStart()
	{
		UpdateSkin();
	}

	public void UpdateSkin()
	{
		if ( Body?.SceneModel is null )
			return;

		Body.SceneModel.SetMaterialOverride( SkinMaterial, "skin" );
		var outfit = Components.Get<Outfit>();
		outfit?.UpdateOutfit();
	}

	public enum AgeType
	{
		Adult,
		Elder
	}

	public enum GenderType
	{
		Masculine,
		Feminine,
		NonBinary
	}

	public enum SkinToneType
	{
		Pale,
		Light,
		Medium,
		Dark
	}
}
