using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RayHitProvider : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster _raycaster;
    
    private readonly List<RaycastResult> _raycastResults = new();

    public bool TryGetHit(Vector2 position, int layer, out GameObject hitObject)
    {
        var pointerData = new PointerEventData(EventSystem.current) { position = position };
        hitObject = null;
        
        _raycaster.Raycast(pointerData, _raycastResults);
        for (var index = 0; index < _raycastResults.Count; index++)
        {
            var result = _raycastResults[index];
            if (result.gameObject.layer != layer)
                continue;
            
            hitObject = result.gameObject;
            break;
        }

        _raycastResults.Clear();
        return hitObject;
    }
}