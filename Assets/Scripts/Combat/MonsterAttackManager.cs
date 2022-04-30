using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MonsterAttackManager : MonoBehaviour
{
    public MonsterAttack currentMonsterAttack;
    public Monster currentMonsterTurn;
    public GameObject currentMonsterTurnGameObject;

    public GameObject currentTargetedMonsterGameObject;
    public Monster currentTargetedMonster;

    public ButtonManagerScript buttonManagerScript;
    public CombatManagerScript combatManagerScript;
    public HUDAnimationManager HUDanimationManager;
    public EnemyAIManager enemyAIManager;
    public UIManager uiManager;
    public MessageManager CombatLog;
    public SoundEffectManager soundEffectManager;

    public TextMeshProUGUI currentMonsterAttackDescription;
    public Image TextBackImage;
    public Image TextBackImageBorder; // temporary

    public GameObject monsterAttackMissText;
    public GameObject monsterCritText;

    public Vector3 cachedTransform;
    public float cachedDamage;

    public float calculatedDamage = 0;
    public float matchingElementBoost = 0;

    public bool dontDealDamage = false;

    public AudioClip CritSound;
    public AudioClip HitSound;
    public AudioClip MissSound; 

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponents();
    }

    // This functions initializes the gameObject's components
    public void InitializeComponents()
    {
        buttonManagerScript = GetComponent<ButtonManagerScript>();
        combatManagerScript = GetComponent<CombatManagerScript>();
        HUDanimationManager = GetComponent<HUDAnimationManager>();
        enemyAIManager = GetComponent<EnemyAIManager>();
        CombatLog = GetComponent<MessageManager>();
        uiManager = GetComponent<UIManager>();
        soundEffectManager = GetComponent<SoundEffectManager>();

        currentMonsterAttackDescription.gameObject.SetActive(false);
        TextBackImage.enabled = false;
        TextBackImageBorder.enabled = false;
    }

    // This function assigns the monster attack that is connected to the pressed button
    public void AssignCurrentButtonAttack(string buttonNumber)
    {
        currentMonsterAttack = buttonManagerScript.ListOfMonsterAttacks[int.Parse(buttonNumber)];
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)
        combatManagerScript.CurrentMonsterAttack = currentMonsterAttack;

        if (currentMonsterAttack.attackOnCooldown)
        {
            uiManager.EditCombatMessage($"{currentMonsterAttack.monsterAttackName} is on cooldown!");
            return;
        }

        combatManagerScript.TargetingEnemyMonsters(true);
        UpdateCurrentTargetText();

        buttonManagerScript.HideAllButtons("AttacksHUDButtons");
        buttonManagerScript.ShowButton("ConfirmButton");

        SetMonsterAttackDescriptionText(currentMonsterAttack.monsterAttackType);
    }

    // This function sets the monster attack descrption box text
    public void SetMonsterAttackDescriptionText(MonsterAttack.MonsterAttackType attackType)
    {
        switch (attackType) {

            case (MonsterAttack.MonsterAttackType.SelfTarget):
                // Fallthrough

            case (MonsterAttack.MonsterAttackType.AllyTarget):
                currentMonsterAttackDescription.gameObject.SetActive(true);
                currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
                    $"\n{currentMonsterAttack.monsterAttackDescription}" +
                    $"\nBuff Type: ({currentMonsterAttack.monsterAttackType.ToString()}) | Accuracy: {currentMonsterAttack.monsterAttackAccuracy}%" +
                    $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
                TextBackImage.enabled = true;
                TextBackImageBorder.enabled = true;
                break;

            default:
                currentMonsterAttackDescription.gameObject.SetActive(true);
                currentMonsterAttackDescription.text = ($"{currentMonsterAttack.monsterAttackName}" +
                    $"\n{currentMonsterAttack.monsterAttackDescription}" +
                    $"\nBase Power: {currentMonsterAttack.monsterAttackDamage} ({currentMonsterAttack.monsterAttackType.ToString()}) | Accuracy: {currentMonsterAttack.monsterAttackAccuracy}%" +
                    $"\nElement: {currentMonsterAttack.monsterAttackElement.ToString()}");
                TextBackImage.enabled = true;
                TextBackImageBorder.enabled = true;
                break;
        }
    }

    // This function updates the targeted enemy text on screen
    public void UpdateCurrentTargetText()
    {
        // Is the monster targeting itself?
        if (combatManagerScript.CurrentMonsterTurn == combatManagerScript.CurrentTargetedMonster)
        {
            //Debug.Log("I got called!");
            HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} on self?");
        }
        else
        {
            //Debug.Log("I got called!");
            HUDanimationManager.MonsterCurrentTurnText.text = ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name} " +
                $"will use {ReturnCurrentButtonAttack().monsterAttackName} " +
                $"on {combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference.name}?");
        }
    }

    // This function assigns the monster attack that is connected to the pressed button
    public MonsterAttack ReturnCurrentButtonAttack()
    {
        return currentMonsterAttack;
    }

    // This function serves as a reset to visual elements on screen (monster attack description)
    public void ResetHUD()
    {
        currentMonsterAttackDescription.text = "";
        TextBackImage.enabled = false;
        TextBackImageBorder.enabled = false;

        currentMonsterAttackDescription.gameObject.SetActive(false);
        combatManagerScript.monsterTargeter.SetActive(false);
    }

    // This function uses the selected monster attack on the selected monster
    public void UseMonsterAttack()
    {
        dontDealDamage = false;

        currentMonsterTurnGameObject = combatManagerScript.CurrentMonsterTurn;
        currentMonsterTurn = combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference; // unrelated to UI popup (missing reassignment calls?)

        // take away action
        currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterActionAvailable = false;
        // does attack have a cooldown? if so, activate it
        if (currentMonsterAttack.attackHasCooldown)
        {
            currentMonsterAttack.attackOnCooldown = true;
            currentMonsterAttack.attackCurrentCooldown = currentMonsterAttack.attackBaseCooldown;
        }

        // First check if the move should deal damage or not
        if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.AllyTarget || currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.SelfTarget)
        {
            currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
            currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

            // Are you trying to target the wrong target?
            if (currentMonsterAttack.monsterAttackType == MonsterAttack.MonsterAttackType.SelfTarget && currentTargetedMonsterGameObject != currentMonsterTurnGameObject)
            {
                uiManager.EditCombatMessage($"{currentMonsterAttack.monsterAttackName} can only be used on self!");
                return;
            }

            if (CheckAttackHit(true))
            {
                dontDealDamage = true;
                QueueAttackSound();
                soundEffectManager.BeginSoundEffectQueue();
                UpdateHUDElements();

                combatManagerScript.CurrentMonsterTurnAnimator.SetBool("buffAnimationPlaying", true);

                // Send log message
                // targeting self?
                if (currentMonsterTurn == currentTargetedMonster)
                {
                    CombatLog.SendMessageToCombatLog($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} used {currentMonsterAttack.monsterAttackName} " +
                        $"on itself!");
                }
                else
                {
                    CombatLog.SendMessageToCombatLog($"{currentMonsterTurn.aiType} {currentMonsterTurn.name} used {currentMonsterAttack.monsterAttackName} " +
                        $"{currentTargetedMonster.aiType} {currentTargetedMonster.name}!");
                }

                Monster monsterWhoUsedAttack = currentMonsterTurn;
                currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats();

                TriggerPostAttackEffects(monsterWhoUsedAttack);

                combatManagerScript.monsterTargeter.SetActive(false);
                combatManagerScript.targeting = false;

                combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
            }
            else
            {
                dontDealDamage = true;

                QueueAttackSound();
                soundEffectManager.BeginSoundEffectQueue();
                UpdateHUDElements();
                AttackMissed();
            }
        }

        if (!dontDealDamage)
        {
            combatManagerScript.CurrentMonsterTurnAnimator.SetBool("attackAnimationPlaying", true);
            QueueAttackSound();
            UpdateHUDElements();
        }
    }

    // Update HUD elements
    public void UpdateHUDElements()
    {
        buttonManagerScript.HideAllButtons("All");
        ResetHUD();
        uiManager.EditCombatMessage();
    }

    // Queue Attack Sound
    public void QueueAttackSound()
    {
        AudioClip monsterAttackSound = currentMonsterAttack.monsterAttackSoundEffect;
        soundEffectManager.AddSoundEffectToQueue(monsterAttackSound);
    }

    // trigger attack effects
    public void TriggerPostAttackEffects(Monster monsterWhoUsedAttack)
    {
        // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
        if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
        {
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.PostAttack)
                {
                    effect.TriggerEffects(this);
                }
            }
        }
    }

    // pre attack function
    public void PreAttackEffectCheck()
    {
        // Pre Attack Effects Go Here
        foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
        {
            if (effect.effectTime == AttackEffect.EffectTime.PreAttack)
            {
                effect.TriggerEffects(this);
            }
        }
    }

    // This function uses the current move to deal damage to the target (should be called after attack animation ends)
    public void DealDamage()
    {
        PreAttackEffectCheck();

        currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        if (currentTargetedMonsterGameObject != null)
        {
            cachedTransform = currentTargetedMonsterGameObject.transform.position;
            cachedTransform.y += 1;
        }

        if (CheckAttackHit(false))
        {
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            currentTargetedMonster.health -= calculatedDamage;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

            currentTargetedMonsterGameObject.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);

            soundEffectManager.AddSoundEffectToQueue(HitSound);
            soundEffectManager.BeginSoundEffectQueue();

            Monster monsterWhoUsedAttack = currentMonsterTurn;
            monsterWhoUsedAttack.health = currentMonsterTurn.health;

            TriggerPostAttackEffects(monsterWhoUsedAttack);

            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
            currentTargetedMonsterGameObject.GetComponent<CreateMonster>().UpdateStats();
            combatManagerScript.monsterTargeter.SetActive(false);
            combatManagerScript.targeting = false;

            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
        else
        {
            // Don't advance turn
            AttackMissed(true);

            // Main target miss, damage others clasue for multi target attacks
            foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
            {
                if (effect.effectTime == AttackEffect.EffectTime.PostAttack)
                {
                    if (effect.typeOfEffect == AttackEffect.TypeOfEffect.DamageAllEnemies)
                    {
                        effect.TriggerEffects(this);
                    }
                }
            }

            // Invoke after
            combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
        }
    }

    // Attack missed function
    public void AttackMissed()
    {
        CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");

        soundEffectManager.AddSoundEffectToQueue(MissSound);
        soundEffectManager.BeginSoundEffectQueue();

        monsterAttackMissText.SetActive(true);
        monsterAttackMissText.transform.position = cachedTransform;
        combatManagerScript.Invoke("NextMonsterTurn", 0.25f);
    }

    // Attack missed function override
    public void AttackMissed(bool DontAdvanceTurn)
    {
        CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");

        soundEffectManager.AddSoundEffectToQueue(MissSound);
        soundEffectManager.BeginSoundEffectQueue();

        monsterAttackMissText.SetActive(true);
        monsterAttackMissText.transform.position = cachedTransform;
    }

    // This function checks accuracy and evasion
    public bool CheckAttackHit(bool selfTargeting)
    {
        float hitChance = 0f;

        // don't use evasion as a stat when self-targeting
        if (!selfTargeting)
        {
            hitChance = (currentMonsterAttack.monsterAttackAccuracy - currentTargetedMonster.evasion) / 100;
        }
        else
        {
            hitChance = (currentMonsterAttack.monsterAttackAccuracy / 100);
        }

        float randValue = Random.value;

        if (randValue < hitChance)
        {
            return true;
        }

        return false;
    }

    // This function checks crit chance
    public bool CheckAttackCrit()
    {
        float critChance = currentMonsterAttack.monsterAttackCritChance / 100;
        float randValue = Random.value;

        if (randValue < critChance)
        {
            return true;
        }

        return false;
    }

    // This function returns the current monster's highest attack stat for split or true damage
    MonsterAttack.MonsterAttackType ReturnMonsterHighestAttackStat(Monster currentMonster)
    {
        if (currentMonster.physicalAttack > currentMonster.magicAttack)
        {
            return MonsterAttack.MonsterAttackType.Physical;
        }
        else
        {
            return MonsterAttack.MonsterAttackType.Magical;
        }
    }

    // This function returns a damage number based on attack, defense + other calcs
    public float CalculatedDamage(Monster currentMonster, MonsterAttack monsterAttack)
    {
        calculatedDamage = 0;
        matchingElementBoost = 0;
        MonsterAttack.MonsterAttackType highestStatType;

        #region This Code Was Moved Lower
        /*
        // First check if attack element matches monster's element. If so, + flat bonus damage by 20% of calculated damage
        if (monsterAttack.monsterAttackElement == currentMonster.monsterType)
        {
            matchingElementBoost = (currentMonsterAttack.monsterAttackDamage * .20f);
        }
        */
        #endregion

        // Calculate resistances
        switch (monsterAttack.monsterAttackType)
        {
            case (MonsterAttack.MonsterAttackType.Magical):
                calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.magicDefense;
                break;

            case (MonsterAttack.MonsterAttackType.Physical):
                calculatedDamage = currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.physicalDefense;
                break;

            case (MonsterAttack.MonsterAttackType.True):
                highestStatType = ReturnMonsterHighestAttackStat(currentMonster);
                if (highestStatType == MonsterAttack.MonsterAttackType.Magical)
                {
                    calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f));
                }
                else
                {
                    calculatedDamage = currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f));
                }
                break;

            case (MonsterAttack.MonsterAttackType.Split):
                highestStatType = ReturnMonsterHighestAttackStat(currentMonster);
                if (highestStatType == MonsterAttack.MonsterAttackType.Magical)
                {
                    calculatedDamage = currentMonster.magicAttack + (currentMonster.magicAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.magicDefense;
                }
                else
                {
                    calculatedDamage = currentMonster.physicalAttack + (currentMonster.physicalAttack * Mathf.RoundToInt(matchingElementBoost + currentMonsterAttack.monsterAttackDamage * .1f)) - currentTargetedMonster.physicalDefense;
                }
                break;

            default:
                Debug.Log("Missing attack or attack type reference?", this);
                break;
        }

        // Check if resistances are TOO high or base damage TOO low; if so, make the base damage at minimum 1% of their max health
        if (calculatedDamage <= Mathf.RoundToInt(currentTargetedMonster.maxHealth * 0.05f))
        {
            Debug.Log("Stat check resistance buff!");
            calculatedDamage = Mathf.RoundToInt(currentTargetedMonster.maxHealth * 0.05f);
        }

        // Check for additional bonus damage
        calculatedDamage += monsterAttack.monsterAttackFlatDamageBonus;

        // Check if attack element matches monster's element. If so, + flat bonus damage by 25% of calculated damage
        if (monsterAttack.monsterAttackElement == currentMonster.monsterType)
        {
            calculatedDamage += Mathf.RoundToInt(calculatedDamage * .25f);
        }

        // Now check for critical hit
        if (CheckAttackCrit())
        {
            calculatedDamage *= 2f;
            CombatLog.SendMessageToCombatLog($"Critical Hit!!! {currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
                $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");

            monsterCritText.SetActive(true);
            monsterCritText.transform.position = cachedTransform;

            cachedDamage = calculatedDamage;
            soundEffectManager.AddSoundEffectToQueue(CritSound);

            currentMonsterTurnGameObject.GetComponent<CreateMonster>().monsterCriticallyStrikedThisRound = true;

            return calculatedDamage;
        }

        // Send log message
        CombatLog.SendMessageToCombatLog($"{currentMonster.aiType} {currentMonster.name} used {currentMonsterAttack.monsterAttackName} " +
            $"on {currentTargetedMonster.aiType} {currentTargetedMonster.name} for {calculatedDamage} damage!");

        cachedDamage = calculatedDamage;
        return calculatedDamage;
    }

    // This function uses the current move to deal damage to the other targets (should be called initial hit)
    public void DealDamageOthers(GameObject otherSpecificTarget)
    {
        //yield return new WaitForSeconds(0);

        currentTargetedMonster = otherSpecificTarget.GetComponent<CreateMonster>().monsterReference;
        Monster monster = otherSpecificTarget.GetComponent<CreateMonster>().monsterReference;

        // Pre Attack Effects Go Here 
        foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
        {
            if (effect.effectTime == AttackEffect.EffectTime.PreAttack)
            {
                effect.TriggerEffects(this);
            }
        }

        //currentTargetedMonster = combatManagerScript.CurrentTargetedMonster.GetComponent<CreateMonster>().monsterReference;
        //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;

        if (otherSpecificTarget != null)
        {
            cachedTransform = otherSpecificTarget.transform.position;
            cachedTransform.y += 1;
        }

        if (CheckAttackHit(false))
        {
            float calculatedDamage = CalculatedDamage(combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference, currentMonsterAttack);
            monster.health -= calculatedDamage;
            otherSpecificTarget.GetComponent<CreateMonster>().monsterDamageTakenThisRound += calculatedDamage;

            otherSpecificTarget.GetComponent<CreateMonster>().UpdateStats();
            //Debug.Log("You've reached update stats!");

            otherSpecificTarget.GetComponent<Animator>().SetBool("hitAnimationPlaying", true);

            soundEffectManager.AddSoundEffectToQueue(HitSound);
            soundEffectManager.BeginSoundEffectQueue();

            Monster monsterWhoUsedAttack = currentMonsterTurn;
            monsterWhoUsedAttack.health = currentMonsterTurn.health;

            // Trigger all attack after effects (buffs, debuffs etc.) - TODO - Implement other buffs/debuffs and durations
            if (currentMonsterTurnGameObject != null && monsterWhoUsedAttack.health > 0)
            {
                foreach (AttackEffect effect in currentMonsterAttack.ListOfAttackEffects)
                {
                    // Don't forever trigger damage
                    if (effect.typeOfEffect == AttackEffect.TypeOfEffect.DamageAllEnemies)
                    {
                        continue;
                    }

                    // Don't forever trigger all nerfs
                    if (effect.typeOfEffect == AttackEffect.TypeOfEffect.CripplingFearEffect)
                    {
                        continue;
                    }

                    if (effect.effectTime == AttackEffect.EffectTime.PostAttack)
                    {
                        effect.TriggerEffects(this);
                    }
                }
            }

            //currentTargetedMonsterGameObject = combatManagerScript.CurrentTargetedMonster;
        }
        else
        {
            CombatLog.SendMessageToCombatLog
                ($"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.aiType} " +
                $"{combatManagerScript.CurrentMonsterTurn.GetComponent<CreateMonster>().monsterReference.name}'s " +
                $"{currentMonsterAttack.monsterAttackName} missed {currentTargetedMonster.aiType} {currentTargetedMonster.name}!");

            soundEffectManager.AddSoundEffectToQueue(MissSound);
            soundEffectManager.BeginSoundEffectQueue();

            monsterAttackMissText.SetActive(true);
            monsterAttackMissText.transform.position = cachedTransform;
        }
    }
}