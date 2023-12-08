namespace Sandbox;

public partial class Npc : Component
{
	[Property] public string Id { get; set; }


	protected override void OnStart()
	{
		_npcDb.Add( Id, this );
		GameObject.Tags.Add( "npc" );
	}


}
