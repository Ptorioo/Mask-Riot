using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

public class Arena : MonoBehaviour
{
    [SerializeField]
    private float openY;

    [SerializeField]
    private float closeY;

    [SerializeField]
    private BoxCollider2D[] walls;

    [SerializeField]
    private Transform[] gates;

    [SerializeField]
    private float openWallColliderSize;

    [SerializeField]
    private float closeWallColliderSize;

    private void Awake()
    {
        Assert.IsTrue(gates.Length > 0 , "error , no gate in array.");
    }

    [ContextMenu("關門")]
    public void CloseGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(closeY , 1);
        }

        foreach (var wall in walls)
        {
            var newSize   = wall.size;
            newSize.y = closeWallColliderSize;
            wall.size    = newSize;
        }
    }

    [ContextMenu("開門")]
    public void OpenGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(openY , 1);
        }

        foreach (var wall in walls)
        {
            var newSize   = wall.size;
            newSize.y = openWallColliderSize;
            wall.size    = newSize;
        }
    }

    public void QuickCloseGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(closeY , 0f);
        }

        foreach (var wall in walls)
        {
            var newSize = wall.size;
            newSize.y = closeWallColliderSize;
            wall.size = newSize;
        } 
    }
}