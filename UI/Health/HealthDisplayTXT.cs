using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textHP;
    [SerializeField] RawImage RawImage;
    private int MaxHealth { get; set; }
    private float Health { get; set; }
    private float Warn { get; set; }
    private float Warn2 { get; set; }
    public void SetupHealthUI(int maxHealth, float hp, float warningThresh, float warnThresh2)
    {
        MaxHealth = maxHealth;
        Health = hp;
        Warn = warningThresh;
        Warn2 = warnThresh2;
        textHP.text = $"{Health}";
        RawImage.color = new Color(0.4f, 0, 0.5f, 0.86f);
        textHP.color = new Color(1, 1, 1);
    }
    public void SetHP(float hp)
    {
        Health = hp;
        textHP.text = $"{Health}";
        if (Health <= Warn2)
        {
            if (Health >= Warn)
            {
                RawImage.color = new Color(0.69f, 0.1f, 0.74f, 0.95f);
            }
            else
            {
                RawImage.color = new Color(0.9f, 0.2f, 0.9f, 1f);
            }
        }
        else
        {
            RawImage.color = new Color(0.4f, 0, 0.5f, 0.86f);
        }
        textHP.color = new Color(1, 1, 1);
    }
}
