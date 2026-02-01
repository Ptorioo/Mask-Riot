#region

using System;
using UnityEngine;

#endregion

namespace Managers
{
    public class BattleHandler : MonoBehaviour
    {
    #region Private Variables

        private PlayerController player;

    #endregion

    #region Unity events

        private void Start()
        {
            player                 =  FindFirstObjectByType<PlayerController>();
            player.DieEventHandler += OnPlayerDieEventHandler;
        }

    #endregion

    #region Private Methods

        private void OnDestroy()
        {
            if (player == null) return;
            player.DieEventHandler -= OnPlayerDieEventHandler;
        }

        private void OnPlayerDieEventHandler(object sender , EventArgs e)
        {
            FindFirstObjectByType<GameOverUI>(FindObjectsInactive.Include).Show();
        }

    #endregion
    }
}