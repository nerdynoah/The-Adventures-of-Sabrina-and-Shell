using TMPro;
using UnityEngine;

public class TextUIStats : MonoBehaviour
{

    [SerializeField] TMP_Text Text;
    protected string InitialText { get; set; } = "";
    protected int Value { get; set; } = 100;
    protected string Extra { get; set; } = "";


    public void SetupText(string start, string extra, int value)
    {
        InitialText = start;
        Extra = extra;
        Value = value;
        ApplyText();

    }
    public void SetValue(int value)
    {
        Value = value;
        ApplyText();
    }

    public void ApplyText()
    {
        Text.text = $"{InitialText}{Value}";
    }


}
