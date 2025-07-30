using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TowerAnimator : ITowerAnimator
{
    private Sequence _sequence;
    private CanvasScaleProvider _scaleProvider;
    
    public bool IsAnimationPlaying => _sequence != null && _sequence.IsActive();

    public TowerAnimator(CanvasScaleProvider scaleProvider)
    {
        _scaleProvider = scaleProvider;
    }
    
    public void PlayAddAnimation(Element element, Vector2 dropPosition, Action onComplete = null)
    {
        var jumpPower = _scaleProvider.Scale.y * 150f;
        
        _sequence?.Kill();
        _sequence = DOTween.Sequence()
            .Append(element.RectTransform.DOJump(dropPosition, jumpPower, 1, 0.5f))
            .OnComplete(() => onComplete?.Invoke());
    }
    
    public void PlayMissAnimation(Element element, Vector2 dropPosition, Action onComplete = null)
    {
        element.RectTransform.position = dropPosition;
        element.PlayDisappearAnimation(onComplete);
    }

    public void PlayRearrangeAnimation(IReadOnlyList<Element> elements, int startIndex, Action onComplete = null)
    {
        _sequence?.Kill();
        _sequence = DOTween.Sequence();
        
        var animationDuration = 0.5f;
        for (var index = startIndex; index < elements.Count - 1; index++)
        {
            var element = elements[index];
            var nextElement = elements[index + 1];
            var newPosition = element.RectTransform.localPosition;
            _sequence.Join(nextElement.RectTransform.DOLocalMove(newPosition, animationDuration).SetEase(Ease.InBack));
            animationDuration += 0.1f;
        }

        _sequence.OnComplete(() => onComplete?.Invoke());
    }
}
