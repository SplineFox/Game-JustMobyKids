using Zenject;

public class ElementPool : MemoryPool<ElementConfiguration, Element>
{
    protected override void Reinitialize(ElementConfiguration configuration, Element element)
    {
        element.Initialize(configuration);
    }
}