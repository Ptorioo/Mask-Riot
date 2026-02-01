#region

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#endregion

public class GameOverUI : MonoBehaviour
{
#region Private Variables

    private Canvas canvas;

    [SerializeField]
    private Text animText;

    [SerializeField]
    private Button BackToMenuBtn;

#endregion

#region Unity events

    private void Start()
    {
        BackToMenuBtn.onClick.AddListener(OnBackToMenuBtnClicked);
    }

#endregion

#region Public Methods

    public void Show()
    {
        gameObject.SetActive(true);
    }

#endregion

#region Private Methods

    private void OnBackToMenuBtnClicked()
    {
        SceneManager.LoadScene(0); //Goto menu scene
    }

#endregion
}