using UnityEngine;
using UnityEngine.UI;

public class Element : MonoBehaviour
{
    [SerializeField] private Image _image;

    private ElementConfiguration _configuration;

    public void Initialize(ElementConfiguration configuration)
    {
        _configuration = configuration;
        _image.sprite = configuration.Sprite;
    }
}