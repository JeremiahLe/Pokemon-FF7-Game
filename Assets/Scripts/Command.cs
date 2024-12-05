using System;

public abstract class Command
{
    public CommandType CommandType;
    public bool DoesShowCommandButtons;

    public abstract bool IsCommandAvailable();

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
