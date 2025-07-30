using UnityEngine;
using Zenject;
using UniRx;

public class FeedbackController : MonoBehaviour
{
    [SerializeField] private FeedbackMessagePanel _panel;
    
    private ILocalizationService _localizationService;
    private Tower _tower;
    private Hole _hole;

    [Inject]
    public void Construct(ILocalizationService localizationService, Tower tower, Hole hole)
    {
        _localizationService = localizationService;
        _tower = tower;
        _hole = hole;
        
        _tower.OnElementAdded
            .Subscribe(_ => ShowText("element_add"))
            .AddTo(this);
        
        _tower.OnElementMissed
            .Subscribe(_ => ShowText("element_miss"))
            .AddTo(this);
        
        _tower.OnElementDroppedOnMaxTower
            .Subscribe(_ => ShowText("element_drop_max_tower"))
            .AddTo(this);
        
        _hole.OnElementDestroyed
            .Subscribe(_ => ShowText("element_destroy"))
            .AddTo(this);
        
        _hole.OnElementDestroyRejected
            .Subscribe(_ => ShowText("element_destroy_reject"))
            .AddTo(this);
    }

    private void ShowText(string id)
    {
        var text = _localizationService.GetText(id);
        _panel.ShowText(text);
    }
}