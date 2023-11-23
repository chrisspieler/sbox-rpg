namespace Sandbox;

public class HoldingHandState : HandState
{
	public override void OnEnabled() => Initialize();
	public override void OnStart() => Initialize();

	public void Initialize()
	{
		if ( HandModel is not null )
		{
			HandModel.Enabled = true;
			HandModel.SceneObject.RenderLayer = SceneRenderLayer.OverlayWithoutDepth;
		}
		Transform.Position = Camera.Main.Position + Transform.LocalPosition;
	}
}
