using UnityEngine;
using UnityEngine.UI;

public class TitleUICtrl : MonoBehaviour
{
#region Private Variables

    [SerializeField]
    private Button startBtn;

    [SerializeField]
    private Button ExitBtn;

    [SerializeField]
    private EnterGameAnimCtrl egac;

#endregion

#region Unity events

    private void Awake()
    {
        startBtn.onClick.AddListener(StartBtn_OnClick);
        ExitBtn.onClick.AddListener(ExitBtn_OnClick);
    }

#endregion

#region Public Methods

    public void ExitBtn_OnClick()
    {
        Application.Quit();
    }

    public void StartBtn_OnClick()
    {
        egac.EnterGameAnim();
    }

#endregion
}