using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "FlameBodyPassive1", menuName = "Ability Triggers/FlameBodyPassive/1")]
public class FlameBodyPassive1 : IAbilityTrigger
{
    public List<AttackEffect> currentAttackEffectTriggered;

    public override async Task<int> TriggerAbility(Monster targetMonster, GameObject targetMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, bool displayLogMessage)
    {
        await Task.Delay(abilityTriggerDelay);

        foreach (AttackEffect attackEffect in currentAttackEffectTriggered)
        {
            await attackEffect.TriggerEffects(targetMonster, targetMonsterGameObject, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack, displayLogMessage);

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}
