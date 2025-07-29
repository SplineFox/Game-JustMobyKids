using System;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public interface ITowerAnimator
{
    bool IsAnimationPlaying { get; }
    
    void PlayAddAnimation(Element element, Vector2 dropPosition, Action onComplete);
    void PlayMissAnimation(Element element, Vector2 dropPosition, Action onComplete);
    void PlayRearrangeAnimation(IReadOnlyList<Element> elements, int startIndex, Action onComplete);
}