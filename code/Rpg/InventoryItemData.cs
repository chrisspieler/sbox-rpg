namespace Sandbox;

[GameResource("Inventory Item", "item", "An item which may exist in a container or in the world.")]
public class InventoryItemData : GameResource
{
	public string Name { get; set; }
	public float Weight { get; set; }
	public float Value { get; set; }
}
