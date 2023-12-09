using static Sandbox.Clothing;

namespace Sandbox;

public static class ClothingExtensions
{
	//
	// Summary:
	//     Return a list of bodygroups and what their value should be
	public static IEnumerable<(string name, int value)> GetBodyGroups( IEnumerable<Clothing> Clothing )
	{
		BodyGroups mask = Clothing.Select( ( Clothing x ) => x.HideBody ).DefaultIfEmpty().Aggregate( ( BodyGroups a, BodyGroups b ) => a | b );
		yield return ("head", ((mask & BodyGroups.Head) != 0) ? 1 : 0);
		yield return ("Chest", ((mask & BodyGroups.Chest) != 0) ? 1 : 0);
		yield return ("Legs", ((mask & BodyGroups.Legs) != 0) ? 1 : 0);
		yield return ("Hands", ((mask & BodyGroups.Hands) != 0) ? 1 : 0);
		yield return ("Feet", ((mask & BodyGroups.Feet) != 0) ? 1 : 0);
	}
}
