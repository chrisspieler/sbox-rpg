using static Sandbox.Clothing;

namespace Sandbox;

public class Outfit : Component, Component.ExecuteInEditor
{
	[Property] public SkinnedModelRenderer Target { get; set; }
	[Property] public Clothing Legs 
	{
		get => _legs;
		set
		{
			_legs = value;
			UpdateOutfit();
		}
	}
	private Clothing _legs;
	[Property] public Clothing Shirt
	{
		get => _shirt;
		set
		{
			_shirt = value;
			UpdateOutfit();
		}
	}
	private Clothing _shirt;
	[Property] public Clothing Jacket
	{
		get => _jacket;
		set
		{
			_jacket = value;
			UpdateOutfit();
		}
	}
	private Clothing _jacket;

	private List<SceneModel> _editorClothing = new();
	private Dictionary<string, SkinnedModelRenderer> _clothingObjects = new();

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

	private void EditorUpdateOutfit()
	{
		EditorDeleteClothing();

		if ( Target is null || Target.SceneModel is null )
			return;

		var clothing = new List<Clothing>();

		if ( Legs is not null )
			clothing.Add( Legs );

		if ( Shirt is not null )
			clothing.Add( Shirt );

		if ( Jacket is not null )
			clothing.Add( Jacket );

		if ( !clothing.Any() )
			return;

		_editorClothing = Clothing.DressSceneObject( Target.SceneModel, clothing );
	}

	private void RuntimeUpdateOutfit()
	{
		RuntimeDeleteClothing();

		if ( Target is null || Target.SceneModel is null )
			return;

		var clothing = new List<Clothing>();

		if ( Legs is not null )
		{
			clothing.Add( Legs );
			const string bodyGroup = "Legs";
			var legsRenderer = CreateClothes( bodyGroup, Legs );
			_clothingObjects[bodyGroup] = legsRenderer;
		}

		if ( Shirt is not null )
		{
			clothing.Add( Shirt );
			const string bodyGroup = "Chest";
			var shirtRenderer = CreateClothes( bodyGroup, Shirt );
			_clothingObjects[bodyGroup] = shirtRenderer;
		}

		if ( Jacket is not null )
		{
			clothing.Add( Jacket );
			const string bodyGroup = "Chest";
			var jacketRenderer = CreateClothes( bodyGroup, Jacket );
			_clothingObjects[bodyGroup] = jacketRenderer;
		}

		foreach ( (string name, int value) in ClothingExtensions.GetBodyGroups( clothing ) )
		{
			Target.SceneModel.SetBodyGroup( name, value );
		}
	}

	private SkinnedModelRenderer CreateClothes( string name, Clothing clothing )
	{
		var clothingGo = new GameObject( true, name );
		clothingGo.Parent = Target.GameObject;
		clothingGo.Transform.World = Target.Transform.World;
		clothingGo.Tags.Add( "clothing" );
		var renderer = clothingGo.Components.Create<SkinnedModelRenderer>();
		renderer.Model = Model.Load( clothing.Model );
		renderer.BoneMergeTarget = Target;
		return renderer;
	}

	private void EditorDeleteClothing()
	{
		foreach( var model in _editorClothing )
		{
			model.Delete();
		}
		_editorClothing.Clear();
	}

	private void RuntimeDeleteClothing()
	{
		foreach ( var kvp in _clothingObjects )
		{
			var bodyGroup = kvp.Key;
			var renderer = kvp.Value;
			renderer.GameObject.DestroyImmediate();
			if ( Target is not null )
			{
				Target.SceneModel.SetBodyGroup( bodyGroup, 1 );
			}
		}
		_clothingObjects.Clear();
	}


}
