using System.Collections.Generic;
using UnityEngine;

public class TowerDropValidator : ITowerDropValidator
{
    private readonly ITowerCollisionDetector _collisionDetector = new TowerCollisionDetector();

    public bool CanDropElement(IReadOnlyList<Element> elements, Element dropElement, Vector2 dropPosition)
    {
        return elements.Count == 0 || 
               _collisionDetector.TryGetDropCollision(elements, dropElement, dropPosition, out var collision);
    }
}