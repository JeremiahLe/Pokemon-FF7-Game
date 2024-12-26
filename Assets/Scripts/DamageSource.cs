using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

[Serializable]
[CreateAssetMenu(fileName = "New Damage Source", menuName = "Damage Sources")]
public class DamageSource : ScriptableObject
{
    public Object DamageCauser { get; private set; }
    
    [field: SerializeField] public DamageType DamageType { get; private set; }
    [field: SerializeField] public List<DamageScalar> DamageScalars { get; private set; }

    public virtual TargetingData GetTargetingData()
    {
        return new TargetingData();
    }
}

[Serializable]
public class DamageScalar
{
    [field: SerializeField] public Stat ScalingStat;
    [field: SerializeField] public float ScalingMultiplier;
}

public enum TargetingType
{
    Single,
    MultiSelect,
    Adjacent,
    All,
    Random
}

[Serializable]
public struct TargetingData
{
    public TargetingType TargetingType;
    public TargetRestrictions TargetRestrictions;
    public int TargetCount;
    public bool CanTargetDead;

    public TargetingData(TargetingType targetingType, TargetRestrictions targetRestrictions, int targetCount, bool canTargetDead)
    {
        TargetingType = targetingType;
        TargetRestrictions = targetRestrictions;
        TargetCount = targetCount;
        CanTargetDead = canTargetDead;

        if (TargetCount == 0) TargetCount = 1;
    }
}

public enum TargetRestrictions
{
    Any, Self, Allies, Enemies
}

public enum DamageType
{
    Physical, Special, True, NonDamaging
}

