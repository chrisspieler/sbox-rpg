namespace Sandbox;

public class PlayerStateMachine : BaseComponent
{
	[Property] public RpgPlayerController Controller { get; set; }
	public PlayerState NextState { get; private set; }
	public PlayerState CurrentState { get; private set; }
	public PlayerState PreviousState { get; private set; }

	public override void OnStart()
	{
		base.OnStart();

		var activeStates = GameObject.Components
			.OfType<PlayerState>()
			.Where( state => state.Enabled )
			.ToArray();
		// If no states are active...
		if ( !activeStates.Any() )
		{
			// ...get the first state and try to enable it...
			var firstState = GameObject.Components
				.OfType<PlayerState>()
				.FirstOrDefault();
			if ( firstState is not null )
			{
				CurrentState = firstState;
				firstState.Enabled = true;
			}
			// ...or if there are none, throw an exception.
			else
			{
				throw new Exception( $"({GameObject.Name}) State machine has no state components!" );
			}
			return;
		}
		// The first active state is the initial state, the rest are disabled.
		for( int i = 0; i < activeStates.Length; i++ )
		{
			if ( i == 0 )
			{
				CurrentState = activeStates[i];
				continue;
			}
			activeStates[i].Enabled = false;
		}
	}

	public T ChangeState<T>( )
		where T : PlayerState
	{
		var nextState = GameObject.GetComponent<T>( false )
			?? throw new Exception( $"({GameObject.Name} has no state: {TypeLibrary.GetType<T>().Name})" );
		if ( CurrentState is not null )
		{
			PreviousState = CurrentState;
			PreviousState.Enabled = false;
		}
		CurrentState = nextState;
		CurrentState.Enabled = true;
		CurrentState.PreviousState = PreviousState;
		return (T)CurrentState;
	}
}
