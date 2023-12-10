namespace Sandbox;

public partial class Outfit
{
	private void RuntimeUpdateOutfit()
	{
		RuntimeDeleteClothing();

		if ( Target is null || Target.SceneModel is null )
			return;

		foreach( var apparel in EquippedApparel )
		{
			if ( apparel is null )
				continue;

			foreach( var model in apparel.EquippedModels )
			{
				var renderer = CreateClothes( apparel.Name, model );
				_clothingObjects.Add( renderer );
			}

			apparel.HideBodyGroups.ApplyToModel( Target.SceneModel );
		}

	}

	private void RuntimeDeleteClothing()
	{
		foreach ( var renderer in _clothingObjects )
		{
			renderer.GameObject.DestroyImmediate();
		}
		_clothingObjects.Clear();

		if ( Target?.SceneModel is null )
			return;

		ClearBodyGroups();
	}

	private SkinnedModelRenderer CreateClothes( string name, string clothingModel )
	{
		var clothingGo = new GameObject( true, name );
		clothingGo.Parent = Target.GameObject;
		clothingGo.Transform.World = Target.Transform.World;
		clothingGo.Tags.Add( "clothing" );
		var renderer = clothingGo.Components.Create<SkinnedModelRenderer>();
		renderer.Model = Model.Load( clothingModel );
		renderer.BoneMergeTarget = Target;
		return renderer;
	}


}
