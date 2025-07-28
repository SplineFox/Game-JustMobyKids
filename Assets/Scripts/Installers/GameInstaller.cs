using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Configurations")]
    [SerializeField] private GameConfiguration _gameConfiguration;
    [SerializeField] private ElementConfigurationDatabase _elementConfigurationDatabase;

    [Header("Prefabs")]
    [SerializeField] private Element _elementPrefab;
    [SerializeField] private Slot _slotPrefab;
    
    [Header("Services")]
    [SerializeField] private DragDropService _dragDropService;
    
    [Header("Containers")]
    [SerializeField] private SlotContainer _slotContainer;
    
    public override void InstallBindings()
    {
        Container.Bind<GameConfiguration>().FromInstance(_gameConfiguration).AsSingle();
        Container.Bind<ElementConfigurationDatabase>().FromInstance(_elementConfigurationDatabase).AsSingle();
        
        Container.BindMemoryPool<Element, ElementPool>()
            .WithInitialSize(10)
            .FromComponentInNewPrefab(_elementPrefab)
            .UnderTransformGroup("ElementPoolContainer");

        Container.BindFactory<ElementConfiguration, Slot, SlotFactory>()
            .FromComponentInNewPrefab(_slotPrefab);

        Container.Bind<InputService>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<DragDropService>().FromInstance(_dragDropService).AsSingle();
        
        Container.BindInterfacesAndSelfTo<SlotContainer>().FromInstance(_slotContainer).AsSingle();
    }
}