namespace Sandbox;

public class Objective
{
	public Objective() { }
	public Objective( string description, Func<bool> isComplete = null )
	{
		Description = description;
		IsComplete = isComplete ?? (() => false);
	}

	public string Description { get; set; }
	public Func<bool> IsComplete { get; set; } = () => false;
}
