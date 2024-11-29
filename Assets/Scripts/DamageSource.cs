using System;
using Object = UnityEngine.Object;

public class DamageSource
{
    public Object DamageCauser { get; private set; }

    public virtual TargetingData GetTargetingData()
    {
        return new TargetingData();
    }
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
}

public enum TargetRestrictions
{
    Any, Self, Allies, Enemies
}

public enum DamageType
{
    Physical, Special, True
}

[Serializable]
public class BasicAttack : DamageSource
{
    public TargetingData TargetingData;
    public DamageType DamageType;

    public override TargetingData GetTargetingData()
    {
        return TargetingData;
    }
}
