namespace Sandbox;

public static class BodyGroupsExtensions
{
	public static void ApplyToModel( this Clothing.BodyGroups bodyGroups, SceneModel model )
	{
		if ( bodyGroups.HasFlag( Clothing.BodyGroups.Head ) )
			model.SetBodyGroup( "head", 1 );
		if ( bodyGroups.HasFlag( Clothing.BodyGroups.Chest ) )
			model.SetBodyGroup( "Chest", 1 );
		if ( bodyGroups.HasFlag( Clothing.BodyGroups.Hands ) )
			model.SetBodyGroup( "Hands", 1 );
		if ( bodyGroups.HasFlag( Clothing.BodyGroups.Legs ) )
			model.SetBodyGroup( "Legs", 1 );
		if ( bodyGroups.HasFlag( Clothing.BodyGroups.Feet ) )
			model.SetBodyGroup( "Feet", 1 );
	}
}
