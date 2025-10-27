
namespace AdventOfCode.Runtime;

public class Traveller
{
    public Traveller() : this(new Location(0, 0), CardinalDirection.North) { }
    public Traveller(CardinalDirection StartDirection) : this(new Location(0, 0), StartDirection) { }
    public Traveller(Location StartLocation) : this(StartLocation, CardinalDirection.North) { }
    public Traveller(Location StartLocation, CardinalDirection StartDirection)
    {
        this.StartLocation = StartLocation;
        this.StartDirection = StartDirection;

        Location = StartLocation;
        CardinalDirection = StartDirection;

        VisitedLocations[Location] = 1;
    }
    public Location Location { get; set; }
    public CardinalDirection CardinalDirection { get; set; }

    public Location StartLocation { get; set; }
    public CardinalDirection StartDirection { get; set; }

    public long TotalStepsTaken
    {
        get; set;
    }
    public long AttemptedStepsTaken
    {
        get; set;
    }

    public bool StoreVisitedLocations { get; set; }

    public Dictionary<Location, int> VisitedLocations { get; } = [];

    public Dictionary<Location, CardinalDirection> VisitedLocationsForVisOnly { get; } = [];

    public int GetVisitedCount(Location location) => VisitedLocations.TryGetValue(location, out var count) ? count : 0;

    /// <summary>
    /// Walk n steps in the direction specified in CardinalDirection.
    /// </summary>
    /// <param name="steps">The steps to take</param>
    /// <param name="CanStepPred">A handler that is called before step is taken.</param>
    /// <param name="TookStepPred">A handler that is called after the step is taken.</param>
    public WalkResult Walk(int steps = 1, Func<Traveller, StepPred> CanStepPred = null, Func<Traveller, bool> TookStepPred = null)
    {
        // no callbacks, just take n steps in a given direction
        if (!StoreVisitedLocations && CanStepPred == null && TookStepPred == null)
        {
            Location += CardinalDirection.Direction * steps;
            TotalStepsTaken = steps;
            AttemptedStepsTaken = steps;
            return WalkResult.Completed;
        }

        // slower method, call our handlers with each step
        for (int s = 0; s < steps; ++s)
        {
            // we must update direction at each step because our callbacks could have changed it
            var dir = CardinalDirection.Direction;

            var p = CanStepPred?.Invoke(this) ?? StepPred.Continue;
            if (p == StepPred.Cancel) return WalkResult.Cancelled;

            AttemptedStepsTaken += 1;
            if (p == StepPred.TryAgain)
            {
                --s;
                continue;
            }

            TotalStepsTaken += 1;
            Location += dir;

            // always store locations for visualization
            VisitedLocationsForVisOnly[Location] = CardinalDirection;
            
            if (StoreVisitedLocations)
                VisitedLocations[Location] = VisitedLocations.TryGetValue(Location, out var visited) ? visited + 1 : 1;

            if (!(TookStepPred?.Invoke(this) ?? true))
                return WalkResult.Cancelled;
        }
        return WalkResult.Completed;
    }
}
