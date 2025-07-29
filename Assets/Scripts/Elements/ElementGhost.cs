using UnityEngine;
using UnityEngine.UI;

public class ElementGhost : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private Image _image;

    public RectTransform RectTransform => _rectTransform;

    public void Show(Element element)
    {
        _image.sprite = element.Configuration.Sprite;
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}