using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Threading.Tasks;
using System.Linq;

[Serializable]
[CreateAssetMenu(fileName = "RandomSingleTargetEffect", menuName = "AdventureModifierTriggers/RandomSingleTargetEffect")]
public class RandomSingleTargetEffect : IAbilityTrigger
{
    public Modifier adventureModifier;

    public List<AttackEffect> listOfAdventureModifierEffects;

    public override async Task<int> TriggerModifier(CombatManagerScript combatManagerScript, Monster.AIType aiType, AttackEffect.EffectTime effectTime)
    {
        Debug.Log($"Triggering {adventureModifier.modifierName}!");

        List<GameObject> targetMonsterList;

        if (aiType == Monster.AIType.Ally)
            targetMonsterList = combatManagerScript.ListOfAllys;
        else
            targetMonsterList = combatManagerScript.ListOfEnemies;

        GameObject monsterObj = combatManagerScript.GetRandomTarget(targetMonsterList);

        Monster targetMonster = monsterObj.GetComponent<CreateMonster>().monsterReference;

        foreach (AttackEffect modifierEffect in listOfAdventureModifierEffects)
        {
            await modifierEffect.TriggerEffects(targetMonster, monsterObj, combatManagerScript.monsterAttackManager, adventureModifier.modifierName, true);

            await Task.Delay(abilityTriggerDelay);
        }

        await Task.Delay(abilityTriggerDelay);

        return 1;
    }
}
