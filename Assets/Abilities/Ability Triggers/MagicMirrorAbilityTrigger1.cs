using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "MagicMirrorAbilityTrigger", menuName = "Ability Triggers/MagicMirror/1")]
public class MagicMirrorAbilityTrigger1 : IAbilityTrigger
{
    public AttackEffect currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, MonsterAttack attackTrigger, bool displayLogMessage)
    {
        if (attackTrigger.monsterAttackDamageType == MonsterAttack.MonsterAttackDamageType.Magical)
        {
            await Task.Delay(abilityTriggerDelay);

            Debug.Log($"Triggering {targetMonster}'s Magic Mirror Ability! Dealing Damage to {attackTrigger.monsterAttackSource.name}!");

            MonsterAttack damageSource = new MonsterAttack(currentAttackEffectTriggered.name, currentAttackEffectTriggered.elementClass, currentAttackEffectTriggered.effectDamageType, currentAttackEffectTriggered.amountToChange, 1, targetMonster, targetMonsterGameObject);

            await currentAttackEffectTriggered.AffectTargetStatByAnotherStat(attackTrigger.monsterAttackSource, attackTrigger.monsterAttackSourceGameObject, monsterAttackManager, ability.abilityName, damageSource);

            return 1;
        }

        Debug.Log("Did not trigger Ability trigger!");

        return 1;
    }
}