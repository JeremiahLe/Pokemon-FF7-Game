using System;
using UnityEngine;

[Serializable]
public class ModifierEffect
{
    public string EffectName { get; private set; }
    public string Description { get; private set; }

    public IActionEffect ActionEffect;

    public virtual ModifierEffect Initialize(IActionEffect actionEffect)
    {
        if (actionEffect is IModifyStatActionEffect statActionEffect)
        {
            return new StatChangeModifier();
        }

        return this;
    }

    public virtual void ApplyModifier(UnitData unitData)
    {
    }

    public virtual void RemoveModifier(UnitData unitData)
    {
    }
}

public class StatChangeModifier : ModifierEffect
{
    public ModifyStatActionEffect ModifyStatActionEffect;
    
    public override void ApplyModifier(UnitData unitData)
    {
        if (ModifyStatActionEffect.IsFlatAmount)
        {
            unitData.ModifyStatProperty(ModifyStatActionEffect.StatToChange, ModifyStatActionEffect.Amount);
        }
        else
        {
            var baseStat = unitData.GetStatProperty(ModifyStatActionEffect.StatToChange).CurrentBaseStat;
            var amount = baseStat * (ModifyStatActionEffect.Amount / 100f);
            unitData.ModifyStatProperty(ModifyStatActionEffect.StatToChange, amount);
        }
    }
}
