using System.Collections.Generic;
using UnityEngine;

public interface ITowerDropValidator
{
    bool CanDropElement(IReadOnlyList<Element> elements, Element dropElement, Vector2 dropPosition);
}