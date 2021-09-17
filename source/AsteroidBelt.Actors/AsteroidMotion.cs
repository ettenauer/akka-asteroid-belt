using System;

namespace AsteroidBelt.Actors;

public sealed class AsteroidMotion
{
    private readonly double step;
    private readonly int r;
    private readonly int h = 150;
    private readonly int k = 150;

    private double theta;
    private double x;
    private double y;

    public AsteroidMotion()
    {
        var rnd = new Random(Guid.NewGuid().GetHashCode());
        this.step = 2 * Math.PI / rnd.Next(10, 20);
        this.r = rnd.Next(10, 50);
    }

    public (double x, double y) CurrentPosition => (x, y);

    public (double x, double y) Move()
    {
        theta += step;
        if (theta > 2 * Math.PI) theta = step;
        x = Math.Round(h + r * Math.Cos(theta), 2);
        y = Math.Round(k - r * Math.Sin(theta), 2);

        return (x, y);
    }
}

