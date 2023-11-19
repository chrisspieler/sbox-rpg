namespace Sandbox;

public class PlayerState : BaseComponent
{
	protected PlayerStateMachine StateMachine
		=> _stateMachine ??= GetComponent<PlayerStateMachine>();
	private PlayerStateMachine _stateMachine;
	protected RpgPlayerController Controller => StateMachine?.Controller;
	protected GameObject Player => StateMachine?.GameObject?.Parent;
	public PlayerState PreviousState { get; set; }
}
