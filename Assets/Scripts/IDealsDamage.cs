using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

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
    public abstract int ActionPointCost { get; }
}

public static class DealsDamageStaticHelpers
{
    public static string GetAutofilledDescription(IDealsDamage interfaceRef)
    {
        var damageType = interfaceRef.DamageType;
        var affinity = interfaceRef.ActionAffinityClass;
        var targets = GetTargetText(interfaceRef.TargetingData);
        var scalars = GetScalarsText(interfaceRef.DamageScalars);

        return interfaceRef.DamageType switch
        {
            DamageType.Physical => $"Deals {affinity.affinityType} {damageType} damage {scalars} to {targets}",
            DamageType.Special => $"Deals {affinity.affinityType} {damageType} damage {scalars} to {targets}",
            DamageType.True => $"Deals {affinity.affinityType} {damageType} damage {scalars} to {targets}",
            DamageType.NonDamaging => "Does something",
            _ => ""
        };
    }

    public static string GetScalarsText(List<DamageScalar> damageScalars)
    {
        var appendedScalars = new StringBuilder("equal to");

        for (var i = 0; i < damageScalars.Count; i++)
        {
            appendedScalars.Append(" ").Append(damageScalars[i].ScalingMultiplier).Append("%").Append(" ").Append(GetStatText(damageScalars[i].ScalingStat));
            if (i == damageScalars.Count - 1) break;
            appendedScalars.Append(" and");
        }

        return appendedScalars.ToString();
    }
    
    public static string GetStatText(Stat scalingStat)
    {
        return scalingStat switch
        {
            Stat.CurrentHealth => "Current Health",
            Stat.MaxHealth => "Max Health",
            Stat.PhysicalAttack => "Physical Attack",
            Stat.SpecialAttack => "Special Attack",
            Stat.PhysicalDefense => "Physical Defense",
            Stat.SpecialDefense => "Special Defense",
            Stat.Speed => "Speed",
            _ => "None"
        };
    }

    public static string GetTargetText(TargetingData targetingData)
    {
        var text = targetingData.TargetCount == 1 ? "a single {0}." : $"{targetingData.TargetCount} {0}s.";

        var targetType = new Regex(Regex.Escape("{0}"));
        text = targetType.Replace(text, GetTargetTypeText(targetingData.TargetRestrictions));
        
        return text;
    }

    public static string GetTargetTypeText(TargetRestrictions targetRestrictions)
    {
        return targetRestrictions switch
        {
            TargetRestrictions.Any => "target",
            TargetRestrictions.Self => "self",
            TargetRestrictions.Allies => "ally",
            TargetRestrictions.Enemies => "enemy",
            _ => "None"
        };
    }
}