﻿namespace Sandbox;

public class WorldHandAnimator : Component
{
	[Property] public SkinnedModelRenderer Target { get; set; }

	public void WithAllFingerCurl( float value )
	{
		Target.Set( "Index", value );
		Target.Set( "Middle", value );
		Target.Set( "Ring", value );
	}
}
