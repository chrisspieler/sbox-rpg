namespace Sandbox;

public static class BaseComponentExtensions
{
	public static void SetEnabled( this Component component, bool enabled )
	{
		component.Enabled = enabled;
	}
}
