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

			if ( apparel.HideBodyGroups.HasFlag( Clothing.BodyGroups.Head ) )
				Target.SceneModel.SetBodyGroup( "head", 1 );
			if ( apparel.HideBodyGroups.HasFlag( Clothing.BodyGroups.Chest ) )
				Target.SceneModel.SetBodyGroup( "Chest", 1 );
			if ( apparel.HideBodyGroups.HasFlag( Clothing.BodyGroups.Hands ) )
				Target.SceneModel.SetBodyGroup( "Hands", 1 );
			if ( apparel.HideBodyGroups.HasFlag( Clothing.BodyGroups.Legs ) )
				Target.SceneModel.SetBodyGroup( "Legs", 1 );
			if ( apparel.HideBodyGroups.HasFlag( Clothing.BodyGroups.Feet ) )
				Target.SceneModel.SetBodyGroup( "Feet", 1 );
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

		Target.SceneModel.SetBodyGroup( "head", 0 );
		Target.SceneModel.SetBodyGroup( "Chest", 0 );
		Target.SceneModel.SetBodyGroup( "Hands", 0 );
		Target.SceneModel.SetBodyGroup( "Legs", 0 );
		Target.SceneModel.SetBodyGroup( "Feet", 0 );
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
