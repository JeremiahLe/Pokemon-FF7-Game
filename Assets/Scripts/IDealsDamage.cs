using System.Collections.Generic;

public interface IDealsDamage
{
    public abstract string ActionName { get; }
    public abstract string ActionDescription { get; }
    public abstract DamageType DamageType { get; }
    public abstract List<DamageScalar> DamageScalars { get; }
    public abstract TargetingData TargetingData { get; }
    public abstract AffinityClass ActionAffinityClass { get; }
}

public interface IBasicAttack : IDealsDamage
{
    
}

public interface ISpecialAction : IDealsDamage
{
    
}
