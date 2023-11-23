namespace Sandbox;

public static class GameObjectExtensions
{
	public static T GetOrCreateComponent<T>( this GameObject gameObject, bool enabledOnly = true, bool deep = false ) 
		where T : BaseComponent, new()
	{
		if ( !gameObject.TryGetComponent<T>(out var component, enabledOnly, deep) )
		{
			component = gameObject.AddComponent<T>();
		}
		return component;
	}
}
