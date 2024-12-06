using System;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Command
{
    public CommandType CommandType;
    public bool DoesShowCommandButtons;

    public abstract bool IsCommandAvailable();
    public abstract void OnCommandStart(CommandWindow commandWindow);

    public static Command ReturnCommandType(CommandType commandType)
    {
        return commandType switch
        {
            CommandType.Attack => new AttackCommand(),
            CommandType.Action => new ActionCommand(),
            CommandType.Defend => new DefendCommand(),
            CommandType.Item => new ItemCommand(),
            CommandType.Pass => new PassCommand(),
            _ => throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null)
        };
    }
}

public enum CommandType
{
    Attack, Action, Defend, Item, Pass, Back
}

public class AttackCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCommandStart(CommandWindow commandWindow)
    {
        //
    }

    public AttackCommand()
    {
        CommandType = CommandType.Attack;
        DoesShowCommandButtons = true;
    }
}

public class ActionCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCommandStart(CommandWindow commandWindow)
    {
        if (!CombatManagerSingleton.CombatManager()) return;

        var combatManager = CombatManagerSingleton.CombatManager();

        if (!combatManager.CurrentUnitAction) return;

        foreach (var action in combatManager.CurrentUnitAction.SpecialActions)
        {
            var actionButton = Object.Instantiate(commandWindow._commandButtonPrefab, commandWindow._commandsTransform);
            actionButton.SetActive(true);
            var commandButtonComponent = actionButton.AddComponent<CommandWindowButton>();
            commandButtonComponent.SetCachedUnitSpecialAction(action);
            commandWindow._temporaryCommandButtons.Add(actionButton);
        }

        commandWindow.FixSiblingIndex();
    }

    public ActionCommand()
    {
        CommandType = CommandType.Action;
        DoesShowCommandButtons = true;
    }
}

public class DefendCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCommandStart(CommandWindow commandWindow)
    {
        //
    }

    public DefendCommand()
    {
        CommandType = CommandType.Defend;
        DoesShowCommandButtons = false;
    }
}

public class ItemCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCommandStart(CommandWindow commandWindow)
    {
        //
    }
    
    public ItemCommand()
    {
        CommandType = CommandType.Item;
        DoesShowCommandButtons = true;
    }
}

public class PassCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCommandStart(CommandWindow commandWindow)
    {
        //
    }

    public PassCommand()
    {
        CommandType = CommandType.Pass;
        DoesShowCommandButtons = false;
    }
}

public class BackCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCommandStart(CommandWindow commandWindow)
    {
        //
    }
    
    public BackCommand()
    {
        CommandType = CommandType.Back;
        DoesShowCommandButtons = true;
    }
}
