namespace Sandbox;

public class HoldingHandState : HandState
{
	protected override void OnEnabled() => Initialize();
	protected override void OnStart() => Initialize();

	public void Initialize()
	{
		if ( HandModel is not null )
		{
			HandModel.Enabled = true;
			HandModel.SceneObject.RenderLayer = SceneRenderLayer.OverlayWithoutDepth;
		}
		Transform.Position = Scene.Camera.Transform.Position + Transform.LocalPosition;
	}
}
