using UnityEngine;

public abstract class ElementContainer : MonoBehaviour
{
    public abstract void AddElement(Element element);
    public abstract void RemoveElement(Element element);
}
