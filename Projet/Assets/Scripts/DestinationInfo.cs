using Unity.VisualScripting;

public class DestinationInfo
{
    public readonly DistanceInfo distance;
    public readonly TimeInfo timeRemaining;
    public readonly float distanceProgression;

    public DestinationInfo(DistanceInfo distance, TimeInfo timeRemaining, float distanceProgression)
    {
        this.distance = distance;
        this.timeRemaining = timeRemaining;
        this.distanceProgression = distanceProgression;
    }
}