#region

using UnityEngine;

#endregion

namespace Player
{
    [CreateAssetMenu(fileName = "PlayerCharacterData" , menuName = "PlayerCharacterData" , order = 0)]
    public class PlayerCharacterData : ScriptableObject
    {
    #region Public Variables

        public bool isInvincible;

        [Min(0.5f)]
        public float jumpForce = 15f;

        [Min(0.5f)]
        public float moveSpeed = 12f;

        [Min(0)]
        public int atk = 1;

        [Min(1)]
        public int hp;

    #endregion
    }
}