using UnityEngine;

[CreateAssetMenu(fileName = "ElementConfiguration", menuName = "JustMobyKids/ElementConfiguration")]
public class ElementConfiguration : ScriptableObject
{
    [SerializeField] private string _id;
    [SerializeField] private Sprite _sprite;
    
    public string Id => _id;
    public Sprite Sprite => _sprite;
}