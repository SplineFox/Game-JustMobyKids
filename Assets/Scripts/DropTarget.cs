using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class DropTarget : MonoBehaviour, IDropTarget
{
    [SerializeField] private RectTransform _container;
    
    private List<IDragTarget> _items = new();
    private float _towerHeight;
    
    public bool CanDrop(IDragTarget dragTarget, Vector2 dropPosition)
    {
        return _towerHeight < _container.rect.height;
    }

    public void OnDrop(IDragTarget dragTarget, Vector2 dropPosition)
    {
        dragTarget.RectTransform.SetParent(_container, false);

        if (_items.Count > 0)
        {
            var item = _items.Last();
            PlaceOverCube(dragTarget, item, dropPosition);
        }
        else
        {
            PlaceOverBox(dragTarget, dropPosition);
        }
        
        _items.Add(dragTarget);
        _towerHeight += dragTarget.RectTransform.rect.height;
    }

    private void PlaceOverCube(IDragTarget dragTarget, IDragTarget last, Vector2 dropPosition)
    {
        var lastHeight = last.RectTransform.rect.height;
        
        var height = dragTarget.RectTransform.rect.height;
        var width = dragTarget.RectTransform.rect.height;
        
        var position = last.RectTransform.localPosition;
        var xPosOffset = Random.Range(-1f, 1f) * (width / 2f);
        
        var posX = last.RectTransform.localPosition.x + xPosOffset;
        var posY = last.RectTransform.localPosition.y + lastHeight / 2f + height / 2f;
        var posZ = dragTarget.RectTransform.localPosition.z;

        dragTarget.RectTransform.localPosition = new Vector3(posX, posY, posZ);
    }

    private void PlaceOverBox(IDragTarget dragTarget, Vector2 dropPosition)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _container,
            dropPosition,
            null,
            out var localPosition
        );
        
        var parentHeight = _container.rect.height;
        var childHeight = dragTarget.RectTransform.rect.height;
        
        var posX = localPosition.x; 
        var posY = -parentHeight * 0.5f + childHeight * 0.5f;
        var posZ = dragTarget.RectTransform.localPosition.z;

        dragTarget.RectTransform.localPosition = new Vector3(posX, posY, posZ);
    }
}
