using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
    
    public float CurrentActionValue
    {
        get => _currentActionValue;
        internal set => _currentActionValue = Mathf.Clamp(value, 0, value);
    }
    private float _currentActionValue;

    public float ActionsTaken
    {
        get => _actionsTaken;
        set => _actionsTaken = value;
    }
    private float _actionsTaken;

    public float CurrentHealth
    {
        get => _currentHealth;
        internal set => _currentHealth = Mathf.Clamp(value, 0, BaseMaxHealthStat.CurrentTotalStat);
    }
    private float _currentHealth;

    public List<StatProperty> Stats { get; private set; }

    [Header("Combat")]
    public StatProperty BaseMaxHealthStat;
    public StatProperty BaseSpeedStat;
    public StatProperty BasePhysicalAttackStat;
    public StatProperty BaseSpecialAttackStat;
    public StatProperty BasePhysicalDefenseStat;
    public StatProperty BaseSpecialDefenseStat;
    
    private void OnValidate()
    {
        if (!UseUniqueResource)
        {
            UnitPrimaryResource = UnitPrimaryResourceClass.GetUnitPrimaryResourceByClass(UnitBaseCombatClass);
        }
    }

    public void InitializeData()
    {
        var fieldObjs = GetType().GetFields().Select(field => field.GetValue(this)).ToList();

        foreach (var field in fieldObjs)
        {
            if (field == null) continue;
            
            if (field.GetType() == typeof(StatProperty))
                ((StatProperty)field).SetAllStats();
        }

        CurrentHealth = BaseMaxHealthStat.CurrentTotalStat; // TODO: Only if health should default to max
    }
}

[Serializable]
public class StatProperty
{
    [field: SerializeField] public float BaseStat;
    
    public float CurrentBaseStat
    {
        get => _currentBaseStat;
        internal set => _currentBaseStat = Mathf.Clamp(value, 1, value);
    }
    private float _currentBaseStat;

    public float CurrentTotalStat
    {
        get => _currentTotalStat;
        internal set => _currentTotalStat = Mathf.Clamp(value, 1, value);
    }
    private float _currentTotalStat;

    public void SetAllStats()
    {
        CurrentBaseStat = BaseStat;
        CurrentTotalStat = CurrentBaseStat;
    }
}
