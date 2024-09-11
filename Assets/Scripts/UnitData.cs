using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "New Unit Data", menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    [field: SerializeField, Title("ID")] public string UnitName { get; private set; }
    
    [field: SerializeField, PreviewField(150)] public Sprite UnitBaseSprite { get; private set; }
    
    [field: SerializeField] public List<AffinityClass> Affinities { get; private set; }
    
    [field: SerializeField] public UnitClass.UnitCombatClass UnitBaseCombatClass { get; private set; }
    
    [field: SerializeField] public List<WeaponType> EquippableWeapons { get; private set; }
    
    [field: SerializeField, Title("Combat")] public float BaseHealth { get; private set; }
    public float CurrentHealth { get; private set; }
    public float MaxHealth { get; private set; }
}
