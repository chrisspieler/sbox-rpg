using static Sandbox.Clothing;

namespace Sandbox;

public partial class Outfit : Component, Component.ExecuteInEditor
{
	[Property] public SkinnedModelRenderer Target { get; set; }
	[Property] public List<ApparelItemData> EquippedApparel { get; set; } = new();
	
	private List<SceneModel> _editorClothing = new();
	private List<SkinnedModelRenderer> _clothingObjects = new();
	private List<ApparelItemData> _lastOutfitState = new();
	private int _lastClothingHash = 0;

	protected override void OnUpdate()
	{
		EquippedApparel ??= new();

		var currentHash = 0;
		foreach( var apparel in EquippedApparel )
		{
			currentHash = HashCode.Combine( currentHash, apparel?.GetHashCode() );
		}

		if ( currentHash != _lastClothingHash )
		{
			RemoveIncompatibleApparel();
			UpdateOutfit();
		}

		_lastClothingHash = currentHash;
		_lastOutfitState = EquippedApparel.ToList();
	}

	private void RemoveIncompatibleApparel()
	{
		var newApparel = EquippedApparel
			.Except( _lastOutfitState )
			.Where( a => a != null )
			.ToList();
		foreach( var newItem in newApparel )
		{
			EquippedApparel.RemoveAll( x => x != null && !newApparel.Contains( x ) && newItem.Slots.HasFlag( x.Slots ) );
		}

	}

	protected override void OnEnabled()
	{
		UpdateOutfit();
	}

	protected override void OnStart()
	{
		UpdateOutfit();
	}

	protected override void OnDisabled()
	{
		DeleteClothing();
	}

	private void UpdateOutfit()
	{
		if ( GameManager.IsPlaying )
		{
			RuntimeUpdateOutfit();
		}
		else
		{
			EditorUpdateOutfit();
		}
	}

	private void DeleteClothing()
	{
		if ( GameManager.IsPlaying )
		{
			RuntimeDeleteClothing();
		}
		else
		{
			EditorDeleteClothing();
		}
	}
}
