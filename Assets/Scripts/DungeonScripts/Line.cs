using UnityEngine;

public class Line
{
    private Orientation orientation;
    private Vector2Int coordinate;

    public Orientation Orientation { get => orientation; set => orientation = value; }
    public Vector2Int Coordinate { get=> coordinate; set => coordinate = value; }
    public Line(Orientation orientation, Vector2Int coordinate)
    {
        this.orientation = orientation;
        this.coordinate = coordinate;
    }
}

public enum Orientation
{
    Horizontal,
    Vertical
}