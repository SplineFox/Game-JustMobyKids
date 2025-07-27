using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class RayProvider : MonoBehaviour
{
    [SerializeField] private GraphicRaycaster _raycaster;
    
    private readonly List<RaycastResult> _raycastResults = new();

    public bool TryGetHit<T>(Vector2 position, out T hit)
    {
        var pointerData = new PointerEventData(EventSystem.current) { position = position };
        hit = default;
        
        _raycaster.Raycast(pointerData, _raycastResults);
        var result = _raycastResults.Count > 0 && _raycastResults[0].gameObject.TryGetComponent(out hit);
        
        _raycastResults.Clear();
        return result;
    }
}