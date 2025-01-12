using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "NewActionEffect", menuName = "Action Effects/ModifyStat")]
public class ModifyStatActionEffect : ActionEffect, IModifyStatActionEffect
{
    [field: SerializeField] public EffectNeutrality EffectNeutrality {get; private set; }
    [field: SerializeField] public int EffectDuration { get; private set; }
    [field: SerializeField, Range(0, 100)] public float EffectTriggerChance { get; private set; }
    [field: SerializeField] public EffectTargetType EffectTargetType { get; private set; }
    [field: SerializeField] public Stat StatToChange { get; private set; }
    [field: SerializeField] public float Amount { get; private set; }
    [field: SerializeField] public bool IsFlatAmount { get; private set; }

    public override async Task<int> TriggerEffects()
    {
        // Create Modifier
        var effect = new StatChangeModifier();
        effect.ModifyStatActionEffect = this;
        var targetUnits = new List<UnitData>();

        // Add Modifier to targets
        if (EffectTargetType == EffectTargetType.Self)
        {
            targetUnits.Add(CombatManagerSingleton.CombatManager().CurrentUnitAction);
        }
        else
        {
            targetUnits = CombatManagerSingleton.CombatManager().CurrentTargetedUnits.Select(x => x.UnitData).ToList();
        }
        
        if (targetUnits.Count <= 0) return await base.TriggerEffects();
        foreach (var target in targetUnits)
        {
            target.AddAndApplyModifier(effect);
        }
        
        return await base.TriggerEffects();
    }
}

public interface IModifyStatActionEffect
{
    public EffectNeutrality EffectNeutrality {get; }
    public int EffectDuration { get; }
    public float EffectTriggerChance { get; }
    public Stat StatToChange { get; }
    public float Amount { get; }
    public bool IsFlatAmount { get; }
}

public enum EffectTargetType { Self, Target }
