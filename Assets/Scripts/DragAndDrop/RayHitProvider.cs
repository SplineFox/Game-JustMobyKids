using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RayHitProvider : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster _raycaster;
    
    private readonly List<RaycastResult> _raycastResults = new();

    public bool TryGetHit(Vector2 position, out GameObject hitObject)
    {
        var pointerData = new PointerEventData(EventSystem.current) { position = position };
        hitObject = null;
        
        _raycaster.Raycast(pointerData, _raycastResults);
        if (_raycastResults.Count > 0)
            hitObject = _raycastResults[0].gameObject;
        
        _raycastResults.Clear();
        return hitObject;
    }
}