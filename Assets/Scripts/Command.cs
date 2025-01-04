using System;
using Object = UnityEngine.Object;

public abstract class Command
{
    public CommandType CommandType;
    public bool DoesShowCommandButtons;
    public IDealsDamage DamageSource;
    public abstract bool IsCommandAvailable();

    public virtual void OnCommandStart(CommandWindow commandWindow)
    {
        OnCommandConfirmed();
    }

    public static event Action<Command> CommandConfirmed;

    private void OnCommandConfirmed()
    {
        CommandConfirmed?.Invoke(this);
    }

    public static Command ReturnCommandType(CommandType commandType)
    {
        return commandType switch
        {
            CommandType.Attack => new AttackCommand(),
            CommandType.ActionStart => new ActionStartCommand(),
            CommandType.ActionConfirm => new ActionConfirmCommand(),
            CommandType.Defend => new DefendCommand(),
            CommandType.Item => new ItemCommand(),
            CommandType.Pass => new PassCommand(),
            _ => throw new ArgumentOutOfRangeException(nameof(commandType), commandType, null)
        };
    }
}

public enum CommandType
{
    Attack, ActionStart, Defend, Item, Pass, Back, ActionConfirm
}

public class AttackCommand : Command
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

        DamageSource = combatManager.CurrentUnitAction.UnitStaticData.BasicAttack;
            
        base.OnCommandStart(commandWindow);
    }

    public AttackCommand()
    {
        CommandType = CommandType.Attack;
        DoesShowCommandButtons = true;
        DamageSource = null;
    }
}

public class ActionStartCommand : Command
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

        if (commandWindow.TemporaryCommandButtons.Count != 0)
        {
            commandWindow.ToggleTemporaryCommands(true);
            base.OnCommandStart(commandWindow);
            return;
        }

        foreach (var action in combatManager.CurrentUnitAction.UnitStaticData.SpecialActions)
        {
            var actionButton = Object.Instantiate(commandWindow._commandButtonPrefab, commandWindow._commandsTransform);
            actionButton.SetActive(true);
            var commandButtonComponent = actionButton.AddComponent<CommandWindowButton>();

            commandButtonComponent.SetCommand(new ActionConfirmCommand(CommandType.ActionConfirm, DoesShowCommandButtons, action), combatManager.CurrentUnitAction);
            commandButtonComponent.AssignButtonEvent(commandWindow);
            commandWindow.TemporaryCommandButtons.Add(actionButton);
        }

        commandWindow.FixSiblingIndex();

        base.OnCommandStart(commandWindow);
    }

    public ActionStartCommand()
    {
        CommandType = CommandType.ActionStart;
        DoesShowCommandButtons = true;
        DamageSource = null;
    }
    
    public ActionStartCommand(CommandType commandType, bool doesShowCommandButtons, UnitSpecialAction unitSpecialAction)
    {
        CommandType = commandType;
        DoesShowCommandButtons = doesShowCommandButtons;
        DamageSource = unitSpecialAction;
    }
}

public class ActionConfirmCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new NotImplementedException();
    }
    
    public ActionConfirmCommand()
    {
        CommandType = CommandType.ActionConfirm;
        DoesShowCommandButtons = true;
        DamageSource = null;
    }

    public ActionConfirmCommand(CommandType commandType, bool doesShowCommandButtons, UnitSpecialAction unitSpecialAction)
    {
        CommandType = commandType;
        DoesShowCommandButtons = doesShowCommandButtons;
        DamageSource = unitSpecialAction;
    }
}

public class DefendCommand : Command
{
    public override bool IsCommandAvailable()
    {
        throw new System.NotImplementedException();
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
    
    public BackCommand()
    {
        CommandType = CommandType.Back;
        DoesShowCommandButtons = true;
    }
}
