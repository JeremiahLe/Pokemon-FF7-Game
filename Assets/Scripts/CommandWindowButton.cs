using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CommandWindowButton : MonoBehaviour
{
    public Command Command { get; private set; }

    private TextMeshProUGUI _buttonText;
    private Button _button;

    public void OnEnable()
    {
        _buttonText = GetComponentInChildren<TextMeshProUGUI>();
        _button = GetComponent<Button>();
    }

    public void SetCommand(Command command)
    {
        Command = command;

        if (Command.DamageSource == null) return;
        SetButtonText(Command.DamageSource.ActionName);
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
 