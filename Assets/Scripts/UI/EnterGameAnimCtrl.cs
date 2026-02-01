using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EnterGameAnimCtrl : MonoBehaviour
{
#region Private Variables

    [SerializeField]
    private Camera _camera;

    [SerializeField]
    private Canvas canvas;

    [SerializeField]
    private Image[] gate;

#endregion

#region Public Methods

    public void EnterGameAnim()
    {
        canvas             = GetComponent<Canvas>();
        _camera            = Camera.main;
        canvas.worldCamera = _camera;
        float timer = 1f;
        for (int i = 0 ; i < gate.Length ; i++)
        {
            DontDestroyOnLoad(gameObject);
            gate[i]
                   .rectTransform.DOAnchorPosX(0f , timer)
                   .SetEase(Ease.OutBounce)
                   .OnComplete(() =>
                               {
                                   StartCoroutine(OpenGate(timer));
                               });
        }
    }

#endregion

#region Private Methods

    private IEnumerator OpenGate(float time)
    {
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene(1);
        for (int i = 0 ; i < gate.Length ; i++)
        {
            DontDestroyOnLoad(gameObject);
            int dir = i == 0 ? 1 : -1; //left, then right
            gate[i]
                   .rectTransform.DOAnchorPosX(dir * 1080f , time)
                   .SetEase(Ease.Linear)
                   .OnComplete(() =>
                               {
                                   Destroy(gameObject);
                               });
        }
    }

#endregion
}