using DG.Tweening;
using UnityEngine;

public class Trans : MonoBehaviour
{
#region Private Variables

    [SerializeField]
    private SpriteRenderer[] tranList;

#endregion

#region Public Methods

    public void DisableView()
    {
        foreach (var spriteRenderer in tranList)
        {
            spriteRenderer.color = new Color(1 , 1 , 1 , 0);
        }
    }

    public void FadeIn(float duration)
    {
        foreach (var spriteRenderer in tranList)
        {
            spriteRenderer.DOFade(1 , duration);
        }
    }

    public void FadeOut(float duration)
    {
        foreach (var spriteRenderer in tranList)
        {
            spriteRenderer.DOFade(0 , duration);
        }
    }

    public void ShowView()
    {
        foreach (var spriteRenderer in tranList)
        {
            spriteRenderer.color = new Color(1 , 1 , 1 , 1);
        }
    }

#endregion

    public void SetTransparent(float alpha)
    {
        foreach (var spriteRenderer in tranList)
        {
            spriteRenderer.color = new Color(1 , 1 , 1 , alpha);
        }
    }
}