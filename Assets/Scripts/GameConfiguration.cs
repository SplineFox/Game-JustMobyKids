using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "GameConfiguration", menuName = "JustMobyKids/GameConfiguration")]
public class GameConfiguration : ScriptableObject
{
    [SerializeField] private List<ElementConfiguration> _configurations;
    
    public IReadOnlyList<ElementConfiguration> Configurations => _configurations;
}
