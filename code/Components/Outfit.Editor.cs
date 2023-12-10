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

			apparel.HideBodyGroups.ApplyToModel( Target.SceneModel );
		}
	}

	private void EditorDeleteClothing()
	{
		foreach ( var model in _editorClothing )
		{
			model.Delete();
		}
		_editorClothing.Clear();
		ClearBodyGroups();
	}
}
