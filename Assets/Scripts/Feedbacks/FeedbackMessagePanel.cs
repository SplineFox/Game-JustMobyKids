using TMPro;
using DG.Tweening;
using UnityEngine;

public class FeedbackMessagePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

    private Sequence _sequence;

    private void Start()
    {
        _text.rectTransform.localScale = Vector3.zero;
    }
    
    private void OnDestroy()
    {
        _sequence?.Kill();
    }

    public void ShowText(string text)
    {
        _text.text = text;
        _text.rectTransform.localScale = Vector3.zero;
        
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(_text.rectTransform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack))
            .AppendInterval(1f)
            .Append(_text.rectTransform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.Linear));
    }
}