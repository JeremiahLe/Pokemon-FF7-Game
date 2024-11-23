using System;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
[CreateAssetMenu(fileName = "New Unit Data", menuName = "Unit/UnitData")]
public class UnitData : ScriptableObject
{
    [field: SerializeField, Title("ID")] public string UnitName { get; private set; }
    
    [field: SerializeField, PreviewField(150)] public Sprite UnitBaseSprite { get; private set; }
    
    [field: SerializeField] public UnitClass.UnitCombatClass UnitBaseCombatClass { get; private set; }

    [field: SerializeField, EnableIf("UseUniqueResource")] public UnitPrimaryResourceClass.UnitPrimaryResource UnitPrimaryResource { get; private set; }

    [SerializeField] private bool UseUniqueResource;
    
    [field: SerializeField] public List<AffinityClass> Affinities { get; private set; }
    
    [field: SerializeField] public List<WeaponType> EquippableWeapons { get; private set; }
    
    [field: SerializeField, Header("Combat")] public float BaseHealth { get; internal set; }
    public float CurrentHealth
    {
        get => _currentHealth;
        internal set => _currentHealth = Mathf.Clamp(value, 0, MaxHealth);
    }
    private float _currentHealth;

    public float MaxHealth { get; internal set; }
    
    [field: SerializeField] public float BaseSpeed { get; internal set; }
    
    [field: SerializeField] public float CurrentActionValue { get; set; }
    

    private void OnValidate()
    {
        if (!UseUniqueResource)
        {
            UnitPrimaryResource = UnitPrimaryResourceClass.GetUnitPrimaryResourceByClass(UnitBaseCombatClass);
        }
    }
}
