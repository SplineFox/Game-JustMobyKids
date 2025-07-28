using Zenject;

public class ElementPool : MemoryPool<ElementConfiguration, Element>
{
    protected override void Reinitialize(ElementConfiguration configuration, Element element)
    {
        element.Initialize(configuration);
    }

    protected override void OnDespawned(Element item)
    {
        item.gameObject.SetActive(false);
    }

    protected override void OnSpawned(Element item)
    {
        item.gameObject.SetActive(true);
    }
}