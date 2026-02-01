using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace DefaultNamespace
{
    public class HealthBar : MonoBehaviour
    {
    #region Private Variables

        [SerializeField]
        private Image front;

    #endregion

    #region Unity events

        private void Start()
        {
            Assert.IsNotNull(front , "[front] can't null.");
        }

    #endregion

    #region Public Methods

        /// <summary>
        /// 更新目前血條
        /// </summary>
        /// <param name="current">目前HP</param>
        /// <param name="max">最大HP</param>
        public void UpdateHealthBar(int current , int max)
        {
            // Debug.Log($"{current} , {max}"); 
            // current          = Mathf.Min(current , 0);
            // current          = Mathf.Max(max , current);
            // Debug.Log($"{current} , {max}"); 
            front.fillAmount = (float)current / max;
        }

    #endregion
    }
}