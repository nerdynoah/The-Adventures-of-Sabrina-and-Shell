using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Enums;

public class GameStarting : MonoBehaviour
{
    private bool toggle = true;
    private Classes roleClasses = Classes.Scout;
    [SerializeField] TextMeshProUGUI Health;
    [SerializeField] TextMeshProUGUI Speed;
    [SerializeField] TextMeshProUGUI Jump;
    [SerializeField] TextMeshProUGUI Ground;
    [SerializeField] TextMeshProUGUI ResMelee;
    [SerializeField] TextMeshProUGUI ResProjectile;
    [SerializeField] TextMeshProUGUI ResExplosion;
    [SerializeField] TextMeshProUGUI ResMagic;
    [SerializeField] TextMeshProUGUI Weight;
    [SerializeField] MsgBox Credit;
    [SerializeField] GameObject location;

    public void OnClick()
    {
        SetLoadData(false);
        SceneManager.LoadScene("CashCow");
    }
    public void QuitGame()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void ShowCredits()
    {
        Credit.ShowCredits();
    }
    public void OnClass(int classes)
    {
        roleClasses = (Classes)classes;
        SetClassPreGame(roleClasses);
        UpdateText();

    }
    public void UpdateText()
    {
        

    }
    public void Start()
    {
        roleClasses = Classes.Scout;
        SetClassPreGame(Classes.Scout);
        UpdateText();
        SetCameraMode(false);
        Cursor.lockState = CursorLockMode.None;
    }
    public int SpeedStat(float speed)
    {
        return (int)((speed / 4.5f) * 100);
    }
    public int JumpStat(float jump)
    {
        return (int)((jump / 70) * 100);
    }
}
