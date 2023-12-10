namespace Sandbox;

public partial class Outfit
{
	private void EditorUpdateOutfit()
	{
		EditorDeleteClothing();

		if ( Target is null || Target.SceneModel is null )
			return;

		foreach( var apparel in EquippedApparel )
		{
			if ( apparel?.EquippedModels is null )
				continue;

			var world = Target.SceneModel.World;

			foreach( var model in apparel.EquippedModels )
			{
				var loadedModel = Model.Load( model );
				var parent = Target.SceneModel;
				var sceneModel = new SceneModel( world, model, parent.Transform );
				_editorClothing.Add( sceneModel );
				parent.AddChild( "clothing", sceneModel );
				sceneModel.MergeBones( parent );
				sceneModel.Update( 0.1f );
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

	private void EditorDeleteClothing()
	{
		foreach ( var model in _editorClothing )
		{
			model.Delete();
		}
		_editorClothing.Clear();
		Target.SceneModel.SetBodyGroup( "head", 0 );
		Target.SceneModel.SetBodyGroup( "Chest", 0 );
		Target.SceneModel.SetBodyGroup( "Hands", 0 );
		Target.SceneModel.SetBodyGroup( "Legs", 0 );
		Target.SceneModel.SetBodyGroup( "Feet", 0 );
	}
}
