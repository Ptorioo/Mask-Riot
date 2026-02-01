#region

using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;

#endregion

public class Arena : MonoBehaviour
{
#region Private Variables

    private readonly float openYo = 5;

    private readonly float closeY = -0.85f;

    [SerializeField]
    private BoxCollider2D[] walls;

    [SerializeField]
    private Transform[] gates;

    [SerializeField]
    private float openWallColliderSize;

    [SerializeField]
    private float closeWallColliderSize;

#endregion

#region Unity events

    private void Awake()
    {
        Assert.IsTrue(gates.Length > 0 , "error , no gate in array.");
    }

#endregion

#region Public Methods

    [ContextMenu("關門")]
    public void CloseGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(closeY , 1);
        }

        foreach (var wall in walls)
        {
            var newSize = wall.size;
            newSize.y = closeWallColliderSize;
            wall.size = newSize;
        }
    }

    [ContextMenu("開門")]
    public void OpenGate()
    {
        foreach (var gate in gates)
        {
            gate.DOLocalMoveY(openYo , 1);
        }

        foreach (var wall in walls)
        {
            var newSize = wall.size;
            newSize.y = openWallColliderSize;
            wall.size = newSize;
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

#endregion
}