using UnityEngine;

public class CanvasScaleProvider : MonoBehaviour
{
    [SerializeField] private Canvas _canvas;
    
    public Vector3 Scale => _canvas.transform.localScale;
}
