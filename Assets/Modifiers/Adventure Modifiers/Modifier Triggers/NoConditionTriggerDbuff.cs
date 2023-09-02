using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;

[Serializable]
[CreateAssetMenu(fileName = "NoConditionTriggerDebuff", menuName = "AdventureModifierTriggers/NoConditionTriggerDebuffEnemies")]
public class NoConditionTriggerDbuff : IAbilityTrigger
{
    public Modifier adventureModifier;

    public List<AttackEffect> listOfAdventureModifierEffects;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        List<GameObject> targetMonsterList;

        if (aiType == Monster.AIType.Ally)
            targetMonsterList = combatManagerScript.ListOfEnemies;
        else
            targetMonsterList = combatManagerScript.ListOfAllys;

        foreach (GameObject monsterObj in targetMonsterList)
        {
            Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

            foreach (AttackEffect modifierEffect in listOfAdventureModifierEffects)
            {
                await modifierEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName, true);

                await Task.Delay(abilityTriggerDelay);
            }

            await Task.Delay(abilityTriggerDelay);
        }

        return 1;
    }
}