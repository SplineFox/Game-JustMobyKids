using System.Collections.Generic;
using UnityEngine;

public class TowerCollisionDetector : ITowerCollisionDetector
{
    public bool TryGetDropCollision(IReadOnlyList<Element> elements, Element dropElement, Vector2 dropPosition, 
        out Element collidedElement)
    {
        collidedElement = null;
        foreach (var element in elements)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(element.RectTransform, dropPosition))
                continue;
            
            collidedElement = element;
            break;
        }
        
        return collidedElement != null;
    }
}
