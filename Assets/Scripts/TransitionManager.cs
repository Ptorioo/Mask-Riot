using DefaultNamespace;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
#region Private Variables

    [SerializeField]
    private BackgroundTexture backgroundTexture;

    private PlayerHorizontalMovement player;

    private bool  transitionStarted;
    private Arena arena;

    [SerializeField]
    private Trans tran_Center;

    [SerializeField]
    private Trans tran_Right;

    [SerializeField]
    private float fadeDuration = 0.5f;

    [SerializeField]
    private float forceSetPlayerX = 42;

#endregion

#region Unity events

    private void Start()
    {
        player = FindAnyObjectByType<PlayerHorizontalMovement>();
        arena  = FindAnyObjectByType<Arena>();
        tran_Center.DisableView();
        tran_Right.ShowView();
    }

    private void Update()
    {
        HandleTransition();
    }

#endregion

#region Private Methods

    private void HandleTransition()
    {
        if (transitionStarted) return;
        var playerX = player.transform.position.x;
        if (playerX >= 21.7f)
        {
            // if (FindFirstObjectByType<GameManager>().IsLastLevel())
            // {
            //     SceneManager.LoadScene(0);
            //     return;
            // }

            transitionStarted = true;
            arena.QuickCloseGate();
            tran_Center.FadeIn(fadeDuration);
            tran_Right.SetTransparent(0.2f);
            Invoke(nameof(DisableRightView) , 0.2f);
            // tran_Right.FadeOut(fadeDuration);
            Invoke(nameof(OnTransitionEnded) , fadeDuration);
        }
    }

    private void DisableRightView()
    {
        tran_Right.SetTransparent(0);
    }

    private void OnTransitionEnded()
    {
        var playerPos = player.transform.position;
        var newX      = playerPos.x - forceSetPlayerX;
        playerPos.x               = newX;
        player.transform.position = playerPos;
        Invoke(nameof(ResetTrans) , 1f);
        transitionStarted = false;
        backgroundTexture.ForceReset();
        arena.QuickCloseGate();
    }

    private void ResetTrans()
    {
        tran_Center.DisableView();
        tran_Right.ShowView();
    }

#endregion
}