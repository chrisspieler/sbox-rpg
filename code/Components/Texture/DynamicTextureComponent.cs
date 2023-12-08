namespace Sandbox;

public class DynamicTextureComponent : Component
{
	[Property, Range(1, 60, 1)] public float MaxUpdatesPerSecond { get; set; } = 15f;
	public Texture InputTexture { get; private set; }
	public Texture OutputTexture { get; private set; }

	private RealTimeSince _lastTextureUpdate;

	protected override void OnUpdate()
	{
		if ( InputTexture is null || _lastTextureUpdate < 1f / MaxUpdatesPerSecond )
			return;

		_lastTextureUpdate = 0f;
		var effects = GameObject.Components.GetAll<TextureEffectComponent>( FindMode.EnabledInSelf );
		var intermediateTexture = InputTexture;
		foreach ( var effect in effects )
		{
			intermediateTexture = effect.Apply( intermediateTexture );
		}
		OutputTexture = intermediateTexture;
		OnPostEffect();
	}

	/// <summary>
	/// Updates the input texture with the given data, creating a new texture if the
	/// dimensions are different.
	/// </summary>
	public virtual void UpdateTextureData( ReadOnlySpan<byte> span, Vector2 size )
	{
		if ( InputTexture == null || InputTexture.Size != size )
		{
			InitializeTexture( size );
		}
		InputTexture.Update( span, 0, 0, (int)size.x, (int)size.y );
	}

	/// <summary>
	/// Create a new inpuit texture with the given size, disposing the previous texture.
	/// </summary>
	public virtual void InitializeTexture( Vector2 size )
	{
		InputTexture?.Dispose();
		InputTexture = Texture.Create( (int)size.x, (int)size.y, ImageFormat.RGBA8888 )
									.WithName( $"{GameObject.Name}_DynamicTexture" )
									.WithDynamicUsage()
									.WithUAVBinding()
									.Finish();
	}

	public virtual void OnPostEffect() { }
}
