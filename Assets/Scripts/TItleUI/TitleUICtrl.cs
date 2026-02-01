using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleUICtrl : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button ExitBtn;


    private void Awake()
    {
        startBtn.onClick.AddListener(StartBtn_OnClick);
        ExitBtn.onClick.AddListener(ExitBtn_OnClick);
    }
    public void StartBtn_OnClick()
    {
        SceneManager.LoadScene(1);
    }
    public void ExitBtn_OnClick()
    {
        Application.Quit();
    }
}
