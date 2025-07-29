using System.Collections.Generic;
using UnityEngine;

public interface ITowerCollisionDetector
{
    public bool TryGetDropCollision(IReadOnlyList<Element> elements, Element dropElement, Vector2 dropPosition,
        out Element collidedElement);
}