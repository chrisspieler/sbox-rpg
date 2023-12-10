namespace Sandbox;

[GameResource( "Apparel Item", "apparel", "An item which may be worn on the body." )]
public class ApparelItemData : InventoryItemData
{
	[ResourceType("vmdl")]
	public string[] EquippedModels { get; set; }
	public ApparelSlot Slots { get; set; }
	public Clothing.BodyGroups HideBodyGroups { get; set; }
}
