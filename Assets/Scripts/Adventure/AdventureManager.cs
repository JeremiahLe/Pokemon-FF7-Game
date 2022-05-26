using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class AdventureManager : MonoBehaviour
{
    [Title("Pre-Adventure Components")]
    public bool adventureBegin = false;
    public GameObject ConfirmAdventureMenu;
    public SceneButtonManager sceneButtonManager;

    [Title("SFX")]
    public AudioSource GameManagerAudioSource;

    public AudioClip adventureBGM;
    public AudioClip combatBGM;
    public AudioClip bossBGM;

    public AudioClip winBGM;
    public AudioClip defeatBGM;

    [Title("Adventure Components")]
    public GameObject nodeSelectionTargeter;
    public bool AdventureMode = false;
    public string adventureSceneName;
    public Adventure currentSelectedAdventure;

    public GameObject currentSelectedNode;
    public GameObject cachedSelectedNode;
    public CreateNode NodeComponent;
    public Monster adventureBoss;

    public GameObject AdventureMenu;
    public GameObject SubscreenMenu;

    public TextMeshProUGUI routeText;
    public TextMeshProUGUI subScreenMenuText;

    public SubscreenManager subscreenManager;

    public GameObject RewardSlotOne;
    public GameObject RewardSlotTwo;
    public GameObject RewardSlotThree;

    [Title("Adventure - Player Components")]
    public int rerollAmount = 0;
    public int timesRerolled = 0;

    public int playerGold = 0;
    public int playerGoldSpent = 0;

    public int playerMonstersLost = 0;
    public int playerMonstersKilled = 0;

    public List<Monster> ListOfCurrentMonsters;
    public List<Modifier> ListOfCurrentModifiers;

    public List<Monster> ListOfAllMonsters;

    [Title("Other Adventure Modules")]
    public enum RewardType { Monster, Modifier }
    public RewardType currentRewardType;

    public Monster currentHoveredRewardMonster;
    public Modifier currentHoveredRewardModifier;

    [Title("Nodes")]
    public GameObject NodeToReturnTo;

    public List<GameObject> ListOfUnlockedNodes;
    public List<GameObject> ListOfLockedNodes;

    public GameObject[] ListOfAllNodes;

    public bool BossDefeated = false;
    public bool BossBattle = false;

    public bool adventureFailed = false;

    [Title("Rewards")]
    public List<Monster> ListOfAvailableRewardMonsters;
    public List<Modifier> ListOfAvailableRewardModifiers;

    public List<Modifier> DefaultListOfAvailableRewardModifiers;

    [Title("Pre-Battle Setup")]
    public int randomBattleMonsterCount;
    public int randomBattleMonsterLimit;

    [Title("Adventure - Battle Setup and Components")]
    public GameObject CombatManagerObject;
    public CombatManagerScript combatManagerScript;

    public List<Monster> ListOfEnemyBattleMonsters;
    public List<Monster> ListOfAllyBattleMonsters;
    public List<Modifier> ListOfEnemyModifiers;

    public void Start()
    {
    }

    public void Awake()
    {
        sceneButtonManager = GetComponent<SceneButtonManager>();
        GameManagerAudioSource = GetComponent<AudioSource>();
        CopyDefaultModifierList();
    }
    
    //
    public void CopyDefaultModifierList()
    {
        // Copy list
        foreach (var item in ListOfAvailableRewardModifiers)
        {
            DefaultListOfAvailableRewardModifiers.Add(new Modifier
            {
                modifierName = item.modifierName,
                modifierAdventureReference = item.modifierAdventureReference,
                adventureModifier = item.adventureModifier,
                modifierAdventureCallTime = item.modifierAdventureCallTime,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified
            });
        }
    }

    //
    public void ResetModifierList()
    {
        ListOfAvailableRewardModifiers.Clear();

        // Copy list
        foreach (var item in DefaultListOfAvailableRewardModifiers)
        {
            ListOfAvailableRewardModifiers.Add(new Modifier
            {
                modifierName = item.modifierName,
                modifierAdventureReference = item.modifierAdventureReference,
                adventureModifier = item.adventureModifier,
                modifierAdventureCallTime = item.modifierAdventureCallTime,
                modifierAmount = item.modifierAmount,
                modifierDescription = item.modifierDescription,
                modifierCurrentDuration = item.modifierCurrentDuration,
                modifierDuration = item.modifierDuration,
                modifierDurationType = item.modifierDurationType,
                modifierRarity = item.modifierRarity,
                modifierSource = item.modifierSource,
                statModified = item.statModified
            });
        }
    }

    // This function enables the confirmAdventureButton after selected adventure
    public void EnableConfirmAdventureButton(Adventure adventureReference)
    {
        ConfirmAdventureMenu.SetActive(true);
        currentSelectedAdventure = adventureReference;
    }

    // This function enables the confirmAdventureButton after selected adventure
    public void DisableConfirmAdventureButton()
    {
        ConfirmAdventureMenu.SetActive(false);
        currentSelectedAdventure = null;
    }

    //
    public void GoToBattleScene()
    {
        DontDestroyOnLoad(gameObject);

        // save nodes
        foreach (GameObject node in ListOfUnlockedNodes)
        {
            node.SetActive(false);
            DontDestroyOnLoad(node);
        }
        //
        foreach (GameObject node in ListOfLockedNodes)
        {
            node.SetActive(false);
            DontDestroyOnLoad(node);
        }
        //
        foreach (GameObject node in ListOfAllNodes)
        {
            //Destroy(node);
        }

        NodeToReturnTo = cachedSelectedNode;
        if (NodeToReturnTo.GetComponent<CreateNode>().nodeType == CreateNode.NodeType.Boss)
        {
            BossBattle = true;
        }

        SceneManager.LoadScene("SetupCombatScene");
    }

    // This function goes to selected adventure scene
    public void GoToAdventureScene()
    {
        switch (currentSelectedAdventure.adventureName)
        {
            case "Basic Adventure":
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                adventureBegin = false;
                sceneButtonManager.GoToSceneCoroutine("BasicAdventureScene");
                break;

            case "Basic Adventure Hard":
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                sceneButtonManager.GoToSceneCoroutine("BasicAdventureHardScene");
                break;

            default:
                break;
        }
    }

    // When adventure scene is loaded, get scene data
    public void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartScreen")
        {
            gameObject.SetActive(false);
            foreach (GameObject node in ListOfAllNodes)
            {
                Destroy(node);
            }
        }
        else if (scene.name == "BasicAdventureScene")
        {
            AdventureMenu = GameObject.FindGameObjectWithTag("AdventureMenu");
            SubscreenMenu = GameObject.FindGameObjectWithTag("SubscreenMenu");
            
            if (SubscreenMenu == null)
            {
                SubscreenMenu = FindInActiveObjectByTag("SubscreenMenu");
            }

            subscreenManager = SubscreenMenu.GetComponent<SubscreenManager>();
            InitiateSceneData();
            adventureSceneName = SceneManager.GetActiveScene().name;

            // Is the boss till alive
            if (!BossDefeated)
            {
                SubscreenMenu.SetActive(false);
            }

            // Check if battle over
            if (!adventureFailed)
            {
                PlayNewBGM(adventureBGM, .60f);
            }
            else
            {
                PlayNewBGM(defeatBGM, .35f);
                ShowFinalResultsMenu(false);
            }
        }
        else
        if (scene.name == "SetupCombatScene")
        {
            CombatManagerObject = GameObject.FindGameObjectWithTag("GameController");
            combatManagerScript = CombatManagerObject.GetComponent<CombatManagerScript>();
            combatManagerScript.adventureMode = true;
            combatManagerScript.previousSceneName = adventureSceneName;

            // Boss Music
            if (BossBattle)
            {
                PlayNewBGM(bossBGM, .35f);
                return;
            }

            // Combat Music
            PlayNewBGM(combatBGM, .30f);
        }
    }

    // This function is called at Game Start, before Round 1, to apply any adventure modifiers
    public void ApplyGameStartAdventureModifiers()
    {
        foreach (Modifier modifier in ListOfCurrentModifiers)
        {
            // Only apply Game Start modifiers
            if (modifier.modifierAdventureCallTime == Modifier.ModifierAdventureCallTime.GameStart)
            {
                // Get specific Modifier
                switch (modifier.modifierAdventureReference)
                {

                    default:
                        break;
                }
            }
        }
    }

    // This function is called every Round Start by CombatManagerScript, to apply any adventure modifiers
    public void ApplyRoundStartAdventureModifiers()
    {
        foreach (Modifier modifier in ListOfCurrentModifiers)
        {
            // Only apply Round Start modifiers
            if (modifier.modifierAdventureCallTime == Modifier.ModifierAdventureCallTime.RoundStart)
            {
                // Get specific Modifier
                switch (modifier.modifierAdventureReference)
                {
                    case Modifier.ModifierAdventureReference.VirulentVenom:

                        // Get random enemy from list of unpoisoned enemies
                        List<GameObject> listOfUnpoisonedEnemies = combatManagerScript.ListOfEnemies.Where(isPoisoned => isPoisoned.GetComponent<CreateMonster>().monsterIsPoisoned == false).ToList();

                        // If no monsters are unpoisoned, break out
                        if (listOfUnpoisonedEnemies.Count == 0)
                        {
                            continue;
                        }

                        // Otherwise, poison randomly selected monster
                        if (listOfUnpoisonedEnemies.Count != 0) {
                            GameObject randomEnemyToPoison = combatManagerScript.GetRandomTarget(listOfUnpoisonedEnemies);
                            Monster monster = randomEnemyToPoison.GetComponent<CreateMonster>().monsterReference;
                            combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} was poisoned by {modifier.modifierName}.");
                            monster.ListOfModifiers.Add(modifier);
                        }

                        // Clear list
                        listOfUnpoisonedEnemies.Clear();
                        break;

                    default:
                        break;
                }
            }
        }
    }

    // This function is called at Game Start by CombatManagerScript as it registers every Monster in Combat, before Round 1
    public void ApplyAdventureModifiers(Monster monster)
    {
        foreach (Modifier modifier in ListOfCurrentModifiers)
        {
            // Get specific Modifier
            switch (modifier.modifierAdventureReference)
            {
                case Modifier.ModifierAdventureReference.WildFervor:
                    monster.critChance += modifier.modifierAmount;
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} critical chance was increased by {modifier.modifierName}.");
                    break;

                case Modifier.ModifierAdventureReference.TemperedOffense:
                    monster.physicalAttack += Mathf.RoundToInt(monster.physicalAttack * (modifier.modifierAmount / 100f) + 1f);
                    monster.magicAttack += Mathf.RoundToInt(monster.magicAttack * (modifier.modifierAmount / 100f) + 1f);
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} physical and magic attack was increased by {modifier.modifierName}.");
                    break;

                case Modifier.ModifierAdventureReference.TemperedDefense:
                    monster.physicalDefense += Mathf.RoundToInt(monster.physicalAttack * (modifier.modifierAmount / 100f) + 1f);
                    monster.magicDefense += Mathf.RoundToInt(monster.magicAttack * (modifier.modifierAmount / 100f) + 1f);
                    combatManagerScript.CombatLog.SendMessageToCombatLog($"{monster.aiType} {monster.name} physical and magic defense was increased by {modifier.modifierName}.");
                    break;

                default:
                    break;
            }
        }
    }

    // 
    public void InitiateSceneData()
    {
        routeText = AdventureMenu.GetComponentInChildren<TextMeshProUGUI>();
        subScreenMenuText = SubscreenMenu.GetComponentInChildren<TextMeshProUGUI>();
        nodeSelectionTargeter = GameObject.FindGameObjectWithTag("Targeter");

        if (adventureBegin == false)
        {
            adventureBoss = currentSelectedAdventure.adventureBoss;
            ListOfAllNodes = GameObject.FindGameObjectsWithTag("Node");
            foreach (GameObject node in ListOfAllNodes)
            {
                node.SetActive(false);
            }

            if (NodeToReturnTo == null)
            {
                foreach (GameObject node in ListOfAllNodes)
                {
                    node.SetActive(true);
                    if (node.GetComponent<CreateNode>().nodeLocked)
                    {
                        ListOfLockedNodes.Add(node);
                    }
                }
            }
        }

        adventureBegin = true;

        foreach (GameObject node in ListOfUnlockedNodes)
        {
            if (node != null)
            {
                node.SetActive(true);
                node.GetComponent<CreateNode>().nodeLocked = false;
                node.GetComponent<SpriteRenderer>().color = Color.white;
            }
        }
        //
        foreach (GameObject node in ListOfLockedNodes)
        {
            if (node != null)
            {
                node.SetActive(true);
                node.GetComponent<CreateNode>().nodeLocked = true;
                node.GetComponent<SpriteRenderer>().color = Color.red;
            }
        }

        //
        if (NodeToReturnTo != null)
        {
            cachedSelectedNode = NodeToReturnTo;
            ActivateNextNode();
        }

    }

    // this function runs on click
    public void CheckNodeLocked()
    {
        if (currentSelectedNode == null)
        {
            return;
        }

        if (currentSelectedNode != null)
        {
            NodeComponent = currentSelectedNode.GetComponent<CreateNode>();
        }

        if (NodeComponent.nodeLocked && !BossDefeated)
        {
            routeText.text = ($"Route is locked!");
            return;
        }

        if (!BossDefeated)
        {
            ShowSubscreenMenu();
        }
    }

    //
    public void ActivateNextNode()
    {
        if (!adventureFailed)
        {
            foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToUnlock)
            {
                node.GetComponent<CreateNode>().nodeLocked = false;
                node.GetComponent<SpriteRenderer>().color = Color.white;
                //node.GetComponent<Animator>().SetBool("unlocked", true);
                ListOfUnlockedNodes.Add(node);
            }
            //
            foreach (GameObject node in cachedSelectedNode.GetComponent<CreateNode>().nodesToLock)
            {
                node.GetComponent<CreateNode>().nodeLocked = true;
                node.GetComponent<SpriteRenderer>().color = Color.red;
                ListOfLockedNodes.Add(node);
            }
        }

        if (BossDefeated)
        {
            routeText.text = ($"You Win!");
            ShowFinalResultsMenu(true);
        }
    }

    //
    public void ShowSubscreenMenu()
    {
        SubscreenMenu.SetActive(true);
        cachedSelectedNode = currentSelectedNode;

        switch (NodeComponent.nodeType)
        {
            case CreateNode.NodeType.Start:
                subScreenMenuText.text = ($"Select starting monster...");
                subscreenManager.LoadRewardSlots(RewardType.Monster);
                break;

            case CreateNode.NodeType.RandomReward:
                currentRewardType = ReturnRandomRewardType();
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(currentRewardType);
                break;

            case CreateNode.NodeType.ModifierReward:
                currentRewardType = RewardType.Modifier;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(RewardType.Modifier);
                break;

            case CreateNode.NodeType.MonsterReward:
                currentRewardType = RewardType.Monster;
                subScreenMenuText.text = ($"Select {currentRewardType.ToString()}...");
                subscreenManager.LoadRewardSlots(RewardType.Monster);
                break;

            case CreateNode.NodeType.RandomCombat:
                subScreenMenuText.text = ($"Select monsters and begin battle...");
                subscreenManager.HideRewardSlots();
                subscreenManager.LoadRandomBattle();
                break;

            case CreateNode.NodeType.Boss:
                subScreenMenuText.text = ($"Select monsters and begin final battle...");
                subscreenManager.HideRewardSlots();
                subscreenManager.LoadRandomBattle();
                break;

            default:
                break;
        }
    }

    //
    public void ShowFinalResultsMenu(bool Win)
    {
        SubscreenMenu.SetActive(true);
        subscreenManager.ShowFinalResultsMenu(Win);
    }

    //
    public Monster GetMVPMonster()
    {
        // Get monster from list with most damage done
        ListOfAllMonsters = ListOfAllMonsters.OrderByDescending(Monster => Monster.cachedDamageDone).ToList();
        Monster mvp = ListOfAllMonsters[0];

        return mvp;
    }

    //
    RewardType ReturnRandomRewardType()
    {
        RewardType randReward = (RewardType)Random.Range(0, 1);
        return randReward;
    }

    //
    GameObject FindInActiveObjectByTag(string tag)
    {

        Transform[] objs = Resources.FindObjectsOfTypeAll<Transform>() as Transform[];
        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].hideFlags == HideFlags.None)
            {
                if (objs[i].CompareTag(tag))
                {
                    return objs[i].gameObject;
                }
            }
        }
        return null;
    }

    //
    public void PlayNewBGM(AudioClip newBGM, float scale)
    {
        GameManagerAudioSource.Stop();
        GameManagerAudioSource.PlayOneShot(newBGM, scale);
    }

}
