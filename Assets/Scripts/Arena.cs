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
    private Transform[] gates;

    private void Awake()
    {
        Assert.IsTrue(gates.Length > 0 , "error , no gate in array.");
    }

    [ContextMenu(nameof(CloseGate))]
    public void CloseGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(closeY , 1);
        }
    }

    [ContextMenu(nameof(OpenGate))]
    public void OpenGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(openY , 1);
        }
    }
}