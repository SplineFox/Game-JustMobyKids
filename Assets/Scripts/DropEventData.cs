using UnityEngine;

public class DropEventData
{
    public Vector2 Position { get; private set; }
    public GameObject GameObject { get; private set; }

    public DropEventData(Vector2 position, GameObject gameObject)
    {
        Position = position;
        GameObject = gameObject;
    }
}