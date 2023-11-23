namespace Sandbox;

public static class GameObjectExtensions
{
	public static T GetOrCreateComponent<T>( this GameObject gameObject, bool startEnabled = true ) 
		where T : BaseComponent, new()
	{
		if ( !gameObject.TryGetComponent<T>(out var component, enabledOnly: false, deep: false ) )
		{
			component = gameObject.AddComponent<T>( startEnabled );
		}
		return component;
	}
}
