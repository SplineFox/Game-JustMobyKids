using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TowerPlacementProvider : ITowerPlacementProvider
{
    private const float PlacementThreshold = 0.25f;
    
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

        var bottomY = - towerRect.rect.height * 0.5f;

        var posX = localPosition.x;
        var posY = bottomY + dropElement.RectTransform.rect.height * 0.5f;
        var posZ = dropElement.RectTransform.position.z;
        
        posX = ClampPositionX(posX, towerRect);
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

        posX = ClampPositionX(posX, towerRect);
        var pos = new Vector3(posX, posY, posZ);
        
        return towerRect.TransformPoint(pos);
    }

    private float ClampPositionX(float value, RectTransform towerRect)
    {
        var widthThreshold = towerRect.rect.width * PlacementThreshold;
        return Mathf.Clamp(value, -widthThreshold, widthThreshold);
    }
}