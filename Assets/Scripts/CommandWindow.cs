using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CommandWindow : MonoBehaviour
{
    [SerializeField] private List<Button> _commandButtons;
    [SerializeField] private Button _backButton;
    
    private Command[] _commandTypes;

    private Command _cachedBackCommand;

    public static event Action<Command> OnCommandButtonClicked;

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
        _commandTypes = new Command[]
            { new AttackCommand(), 
                new ActionCommand(), 
                new DefendCommand(), 
                new ItemCommand(), 
                new PassCommand() };

        _cachedBackCommand = new BackCommand();
        
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
            commandWindowButton.Command = _commandTypes[i];
            button.onClick.AddListener(delegate { CommandButtonClicked(commandWindowButton.Command); });
            i++;
        }
        
        _backButton.onClick.AddListener(delegate
        {
            ToggleAllCommands(true); 
            OnCommandButtonClicked?.Invoke(_cachedBackCommand);
        });
    }

    private void CommandButtonClicked(Command command)
    {
        OnCommandButtonClicked?.Invoke(command);

        if (!command.DoesShowCommandButtons)
        {
            ToggleAllCommands(false, true);
            return;
        }

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
