using System.Threading.Tasks;

namespace Sandbox;

[Title( "Video Player" )]
[Category( "Light" )]
[Icon( "videocam", "red", "white" )]
public class VideoPlayerComponent : Component
{
	[Property]
	public DynamicTextureComponent TextureTarget { get; set; }
	[Property, ResourceType( "mp4" )]
	public string VideoPath { get; set; }
	[Property] public bool Loop 
	{
		get => _loop;
		set
		{
			_loop = value;
			if ( VideoPlayer is not null )
			{
				VideoPlayer.Repeat = value;
			}
		}
	}
	private bool _loop = false;
	protected bool IsInitializing { get; set; }
	public virtual bool VideoLoaded { get; protected set; }
	public virtual bool AudioLoaded { get; protected set; }
	protected VideoPlayer VideoPlayer;
	protected TimeSince VideoLastUpdated { get; set; }

	protected override void OnStart()
	{
		base.OnStart();

		IsInitializing = true;

		PlayFile( VideoPath );
		WaitUntilReady();
	}

	protected virtual void OnTextureData( ReadOnlySpan<byte> span, Vector2 size )
	{
		if ( !VideoLoaded )
			Log.Info( $"Video is now loaded: {size.x}x{size.y}" );

		TextureTarget.UpdateTextureData( span, size );
		VideoLoaded = true;
		VideoLastUpdated = 0;
	}

	protected virtual async Task WaitUntilReady()
	{
		if ( !IsInitializing )
			return;

		while ( !(VideoLoaded && AudioLoaded) )
		{
			await GameTask.DelaySeconds( Time.Delta );
		}
		Log.Info( "Finished initializing" );
		IsInitializing = false;
	}

	protected override void OnUpdate()
	{
		VideoPlayer?.Present();
	}

	public virtual void Stop()
	{
		if ( VideoPlayer == null )
			return;

		// CurrentlyPlayingAudio?.Stop( true );
		AudioLoaded = false;
		VideoPlayer.Stop();
		VideoPlayer.Dispose();
		VideoLoaded = false;
		VideoPlayer = null;
	}

	protected virtual void PlayFile( string filePath )
	{
		VideoPlayer = new VideoPlayer();
		VideoPlayer.OnAudioReady += () =>
		{
			Log.Info( "Audio loaded." );
			AudioLoaded = true;
		};
		VideoPlayer.Repeat = Loop;
		VideoPlayer.OnTextureData += OnTextureData;
		VideoPlayer.Play( FileSystem.Mounted, filePath );
	}
}
