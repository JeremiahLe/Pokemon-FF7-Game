using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandWindowButton : MonoBehaviour
{
    public Command Command { get; private set; }

    private TextMeshProUGUI _buttonText;
    private Button _button;
    
    private UnitData _currentUnit;
    private ISpecialAction _specialAction;

    public void OnEnable()
    {
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponent<Button>();
    }

    private void Update()
    {
        if (!_currentUnit) return;
        if (_specialAction == null) return;

        _button.enabled = _currentUnit.UnitCurrentActionPoints >= _specialAction.ActionPointCost;
    }

    public void SetCommand(Command command, UnitData unitData = null)
    {
        Command = command;
        _currentUnit = unitData;
        
        if (Command.DamageSource == null) return;
        SetButtonText(Command.DamageSource.ActionName);
        _specialAction = (ISpecialAction)Command.DamageSource;
    }

    public void AssignButtonEvent(CommandWindow commandWindow)
    {
        _button.onClick.AddListener(() =>
        {
            Command.OnCommandStart(commandWindow);
            commandWindow.CommandButtonClicked(Command);
        });
    }
    
    public void SetButtonText(string text)
    {
        _buttonText.text = text;
    }
}
 