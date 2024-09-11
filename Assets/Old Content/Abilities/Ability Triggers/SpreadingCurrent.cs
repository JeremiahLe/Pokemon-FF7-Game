using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "SpreadingCurrent", menuName = "Ability Triggers/SpreadingCurrent")]
public class SpreadingCurrent : IAbilityTrigger
{
    public AttackEffect physicalAttackBuffEffect;
    public AttackEffect magicAttackBuffEffect;
    public override async Task<int> TriggerAbility(Monster abilityMonster, GameObject abilityMonsterGameObject, MonsterAttackManager monsterAttackManager, Ability ability, bool displayLogMessage)
    {
        await Task.Delay(abilityTriggerDelay);

        Debug.Log($"Triggering {abilityMonster}'s {ability.abilityName} ability!", this);

        foreach (GameObject enemyMonster in monsterAttackManager.combatManagerScript.ListOfEnemies.ToArray())
        {
            if (enemyMonster == null)
                continue;

            Monster monsterReference = enemyMonster.GetComponent<CreateMonster>().monsterReference;

            // if (monsterReference.monsterAffinity.element == AffinityClass.AffinityType.Time || monsterReference.monsterAffinity.element == AffinityClass.AffinityType.Water ||
            //     monsterReference.monsterSubAffinity.element == AffinityClass.AffinityType.Time || monsterReference.monsterSubAffinity.element == AffinityClass.AffinityType.Water)
            // {
            //     Debug.Log($"Time or Water element monster found!", this);
            //
            //     foreach (GameObject allyMonster in monsterAttackManager.combatManagerScript.ListOfAllys)
            //     {
            //         Monster allyMonsterReference = allyMonster.GetComponent<CreateMonster>().monsterReference;
            //
            //         if (allyMonsterReference.monsterAffinity.element == AffinityClass.AffinityType.Electric 
            //             || allyMonsterReference.monsterSubAffinity.element == AffinityClass.AffinityType.Electric)
            //         {
            //             await Task.Delay(abilityTriggerDelay);
            //
            //             await physicalAttackBuffEffect.TriggerEffects(allyMonsterReference, allyMonster, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);
            //
            //             await Task.Delay(abilityTriggerDelay);
            //
            //             await magicAttackBuffEffect.TriggerEffects(allyMonsterReference, allyMonster, monsterAttackManager, ability.abilityName, monsterAttackManager.currentMonsterAttack);
            //
            //             await Task.Delay(abilityTriggerDelay);
            //         }
            //
            //         await Task.Delay(abilityTriggerDelay);
            //     }
            //
            //     return 1;
            // }
        }

        Debug.Log($"Did not trigger {abilityMonster}'s {ability.abilityName} ability!", this);
        return 1;
    }
}
