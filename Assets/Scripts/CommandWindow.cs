using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandWindow : MonoBehaviour
{
    [SerializeField] private List<Button> _commandButtons;
    [SerializeField] private Button _backButton;
    
    private CommandType[] _commandTypes;

    public static event Action<CommandType> OnCommandButtonClicked;

    private void Awake()
    {
        InitializeButtonComponents();
        InitializeButtonEvents();
    }

    private void OnDestroy()
    {
        UnitObject.OnConfirmTarget -= TargetConfirmed;
        CombatManager.OnUnitActionStart -= UnitActionStart;
    }

    private void InitializeButtonComponents()
    {
        _commandTypes = new[]
            { CommandType.Attack, 
                CommandType.Action, 
                CommandType.Defend, 
                CommandType.Item, 
                CommandType.Pass };
        
        UnitObject.OnConfirmTarget += TargetConfirmed;
        CombatManager.OnUnitActionStart += UnitActionStart;
    }

    private void UnitActionStart(UnitObject unitObject)
    {
        ToggleAllCommands(true);
    }

    private void InitializeButtonEvents()
    {
        var i = 0;
        foreach (var button in _commandButtons)
        {
            var commandWindowButton = button.gameObject.AddComponent<CommandWindowButton>();
            commandWindowButton.CommandType = _commandTypes[i];
            button.onClick.AddListener(delegate { CommandButtonClicked(commandWindowButton.CommandType); });
            i++;
        }
        
        _backButton.onClick.AddListener(delegate
        {
            ToggleAllCommands(true); 
            OnCommandButtonClicked?.Invoke(CommandType.Back);
        });
    }

    private void CommandButtonClicked(CommandType commandType)
    {
        OnCommandButtonClicked?.Invoke(commandType);
        ToggleAllCommands(false);
    }

    private void TargetConfirmed(UnitObject unitObject)
    {
        ToggleAllCommands(false, true);
    } 

    public void ToggleAllCommands(bool setActive, bool hideAllOverride = false)
    {
        if (hideAllOverride) setActive = false;
        
        foreach (var button in _commandButtons)
        {
            button.gameObject.SetActive(setActive);
        }

        if (hideAllOverride) setActive = true;
        
        _backButton.gameObject.SetActive(!setActive);
    }
}
