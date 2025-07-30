using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [Header("Configurations")]
    [SerializeField] private GameConfiguration _gameConfiguration;
    [SerializeField] private LocalizationConfiguration _localizationConfiguration;
    [SerializeField] private ElementConfigurationDatabase _elementConfigurationDatabase;

    [Header("Prefabs")]
    [SerializeField] private Element _elementPrefab;
    [SerializeField] private Slot _slotPrefab;
    
    [Header("Scene dependencies")]
    [SerializeField] private Hole _hole;
    [SerializeField] private Tower _tower;
    [SerializeField] private ElementGhost _elementGhost;
    [SerializeField] private DragDropService _dragDropService;
    
    public override void InstallBindings()
    {
        BindConfigurations();
        BindServices();
        BindFactories();
        BindSceneDependencies();
    }

    private void BindConfigurations()
    {
        Container.Bind<GameConfiguration>().FromInstance(_gameConfiguration).AsSingle();
        Container.Bind<LocalizationConfiguration>().FromInstance(_localizationConfiguration).AsSingle();
        Container.Bind<ElementConfigurationDatabase>().FromInstance(_elementConfigurationDatabase).AsSingle();
    }

    private void BindServices()
    {
        Container.BindInterfacesAndSelfTo<InputService>().FromNew().AsSingle();
        
        Container.BindInterfacesAndSelfTo<SaveService>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<SaveInvoker>().FromNew().AsSingle();
        
        Container.BindInterfacesAndSelfTo<LocalizationLoader>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<LocalizationService>().FromNew().AsSingle();
        Container.BindInterfacesAndSelfTo<LocalizationController>().FromNew().AsSingle();
    }

    private void BindFactories()
    {
        Container.BindMemoryPool<Element, ElementPool>()
            .WithInitialSize(20)
            .FromComponentInNewPrefab(_elementPrefab)
            .UnderTransformGroup("ElementPoolContainer");

        Container.BindFactory<ElementConfiguration, Slot, SlotFactory>()
            .FromComponentInNewPrefab(_slotPrefab);
    }

    private void BindSceneDependencies()
    {
        Container.BindInterfacesAndSelfTo<Hole>().FromInstance(_hole);
        Container.BindInterfacesAndSelfTo<Tower>().FromInstance(_tower);
        Container.BindInterfacesAndSelfTo<ElementGhost>().FromInstance(_elementGhost).AsSingle();
        Container.BindInterfacesAndSelfTo<DragDropService>().FromInstance(_dragDropService).AsSingle();
    }
}