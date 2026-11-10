public enum DistanceScale
{
    Kilometers,
    LightSeconds,
    AstronomicalUnits,
    LightYears,
    Parsec
}

public struct DistanceInfo
{
    public readonly double value;
    public readonly DistanceScale scale;
    public readonly string label;

    public DistanceInfo(double distance)
    {
        (scale, value) = distance switch
        {
            <= 10e3 => (DistanceScale.Kilometers, distance),
            <= 10e7 => (DistanceScale.LightSeconds, distance / 299792458.0),
            <= 10e11 => (DistanceScale.AstronomicalUnits, distance / 149597870700.0),
            <= 10e15 => (DistanceScale.LightYears, distance / 9460730472580800.0),
            _ => (DistanceScale.Parsec, distance / 30856775814913673.0)
        };

        label = distance switch
        {
            <= 10e7 => $"{value:F4} km",
            <= 10e11 => $"{value:F4} ls",
            <= 10e15 => $"{value:F4} au",
            <= 10e18 => $"{value:F4} ly",
            _ => $"{value:F4} pc"
        };
    }
}