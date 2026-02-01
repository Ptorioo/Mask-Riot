using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class TitleUICtrl : MonoBehaviour
{
    [SerializeField] private Button startBtn;
    [SerializeField] private Button ExitBtn;
    [SerializeField] private EnterGameAnimCtrl egac;
    private void Awake()
    {
        startBtn.onClick.AddListener(StartBtn_OnClick);
        ExitBtn.onClick.AddListener(ExitBtn_OnClick);
    }
    public void StartBtn_OnClick()
    {
        egac.EnterGameAnim();

    }
    public void ExitBtn_OnClick()
    {
        Application.Quit();
    }

}
