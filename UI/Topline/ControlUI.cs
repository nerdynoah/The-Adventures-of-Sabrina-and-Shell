using TMPro;
using UnityEngine;

public class ControlUI : MonoBehaviour
{
    [SerializeField] TMP_Text Text;
    protected string Control { get; set; } = "";
    public void SetControl(string value)
    {
        Control = value;
        ApplyText();
    }
    public void ApplyText()
    {
        Text.text = $"{Control}";
    }
}
