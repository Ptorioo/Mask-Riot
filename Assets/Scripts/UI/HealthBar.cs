#region

using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

#endregion

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

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 更新目前血條
        /// </summary>
        /// <param name="current">目前HP</param>
        /// <param name="max">最大HP</param>
        public void UpdateHealthBar(int current , int max)
        {
            // 被打才會顯示血條
            if (gameObject.activeSelf == false) Show();
            front.fillAmount = (float)current / max;
        }

    #endregion
    }
}