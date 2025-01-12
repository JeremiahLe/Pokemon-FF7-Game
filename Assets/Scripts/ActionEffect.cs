using System;
using System.Threading.Tasks;
using UnityEngine;

public interface IActionEffect
{
    public void SetEffectOwner(UnitData unitData);
}

[Serializable]
public class ActionEffectStaticData
{
    [field: SerializeField] public EffectNeutrality EffectNeutrality {get; private set; }
    [field: SerializeField] public EffectTriggerTime EffectTriggerTime { get; private set; }
    [field: SerializeField] public int EffectDuration { get; private set; }
    [field: SerializeField, Range(0, 100)] public float EffectTriggerChance { get; private set; }
}

[Serializable]
public class ActionEffect : ScriptableObject, IActionEffect
{
    [field: SerializeField] public string EffectName { get; private set; }
    [field: SerializeField, TextArea] public string EffectDescription { get; private set; }
    
    [field: SerializeField] public EffectTriggerTime EffectTriggerTime { get; private set; }
    public UnitData ActionEffectOwner { get; private set; }

    public void SetEffectOwner(UnitData unitData)
    {
        ActionEffectOwner = unitData;
    }

    public void SetEffectDescription(string text)
    {
        EffectDescription = text;
    }

    public virtual async Task<int> TriggerEffects()
    {
        return 1;
    }
}

public enum EffectNeutrality { Buff, Debuff, Other }

public enum EffectTriggerTime
{
    BeforeAttack, 
    DuringAttack, 
    AfterAttack, 
    OnKill, 
    OnDeath,
    GameStart,
    RoundStart,
    RoundEnd,
    OnStatChange,
    OnDamageTaken,
    PreOtherAttack,
    OnDamageDealt,
    OnDamageNullified,
    OutOfCombatPassive,
    OnDebuffNullified,
    OnItemUsed
}
