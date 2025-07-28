using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;

    private ElementConfiguration _configuration;
    
    public RectTransform RectTransform => _rectTransform;

    public void Initialize(ElementConfiguration configuration)
    {
        _configuration = configuration;
        _image.sprite = configuration.Sprite;
    }
}