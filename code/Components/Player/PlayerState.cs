namespace Sandbox;

public class PlayerState : Component
{
	protected PlayerStateMachine StateMachine
		=> _stateMachine ??= Components.Get<PlayerStateMachine>();
	private PlayerStateMachine _stateMachine;
	public RpgPlayerController Controller => StateMachine?.Controller;
	public GameObject Player => StateMachine?.GameObject?.Parent;
	public PlayerState PreviousState { get; set; }
	public T ChangeState<T>() where T : PlayerState
		=> StateMachine.ChangeState<T>();
}
