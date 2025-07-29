using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerPlacementProvider : ITowerPlacementProvider
{
    public Vector3 GetPositionForElement(IReadOnlyList<Element> elements, Element dropElement, Vector2 dropPosition, RectTransform towerRect)
    {
        if (elements.Count == 0)
            return GetPositionForFirst(dropElement, dropPosition, towerRect);

        var lastElement = elements.Last();
        return GetPositionForLast(dropElement, lastElement, towerRect);
    }
    
    private Vector3 GetPositionForFirst(Element dropElement, Vector2 dropPosition, RectTransform towerRect)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(towerRect, dropPosition, null,
            out var localPosition
        );

        var widthThreshold = towerRect.rect.width * 0.25f;
        var bottomY = - towerRect.rect.height * 0.5f;

        var posX = Mathf.Clamp(localPosition.x, -widthThreshold, widthThreshold);
        var posY = bottomY + dropElement.RectTransform.rect.height * 0.5f;
        var posZ = dropElement.RectTransform.position.z;
        var pos = new Vector3(posX, posY, posZ);

        return towerRect.TransformPoint(pos);
    }

    private Vector3 GetPositionForLast(Element dropElement, Element lastElement, RectTransform towerRect)
    {
        var lastHeight = lastElement.RectTransform.rect.height;
        
        var height = dropElement.RectTransform.rect.height;
        var width = dropElement.RectTransform.rect.height;

        var posXOffset = Random.Range(-1f, 1f) * (width * 0.5f);
        var posX = lastElement.RectTransform.localPosition.x + posXOffset;
        var posY = lastElement.RectTransform.localPosition.y + (lastHeight + height) * 0.5f;
        var posZ = lastElement.RectTransform.localPosition.z;
        var pos = new Vector3(posX, posY, posZ);
        
        return towerRect.TransformPoint(pos);
    }
}