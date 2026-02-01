using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class DeadUICtrl : MonoBehaviour
{
    [SerializeField] private Text animText;
    [SerializeField] private Button[] btmBtn;
    private Canvas canvas;
    public void ShowDeadUI() => gameObject.SetActive(true);
    private void Awake()
    {
        //canvas = GetComponent<Canvas>();
        //canvas.worldCamera = Camera.main;
        //canvas.sortingLayerID = SortingLayer.NameToID("UI");
        //canvas.sortingLayerName = "UI";
        //canvas.sortingOrder = 10;
        for (int i = 0; i < btmBtn.Length; i++)
        {
            btmBtn[i].onClick.AddListener(BackToMenuBtn_OnClick);
        }
    }
    private void Start()
    {
        StartTextAnim();
    }
    private void OnDestroy()
    {
        for (int i = 0; i < btmBtn.Length; i++)
        {
            btmBtn[i].onClick.RemoveAllListeners();
        }
    }
    public void BackToMenuBtn_OnClick()
    {
        SceneManager.LoadScene(0); //main menu scene
    }
    public void StartTextAnim()
    {
        animText.rectTransform.DOAnchorPos(Vector2.zero, 1.5f).SetEase(Ease.OutQuart);
        animText.DOColor(new Color(1, 1, 1, 1), 1f).SetEase(Ease.OutQuart);
    }
}
