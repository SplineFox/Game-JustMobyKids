using System.Collections.Generic;
using UnityEngine;

public interface ITowerPlacementProvider
{
    public Vector3 GetPositionForElement(IReadOnlyList<Element> elements, Element dropElement, Vector2 dropPosition,
        RectTransform towerRect);
}