using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Lesson_1.Task_1
{
    public class Unit : MonoBehaviour
    {
        #region Fields

        [SerializeField] private Slider _healthBar;
        [SerializeField] private Button _startHealingButton;
        [SerializeField] private Button _resetHealthButton;
        [SerializeField] private Text _healthText;
        [Space]
        [SerializeField] private bool _canRenewHealing;

        private int _health;

        private Coroutine _healing;

        private const int MAX_HEALTH = 100;
        private const int HEALING_AMOUNT = 5;
        private const float HEALING_INTERVAL = 0.5f;
        private const float HEALING_DURATION = 3.0f;

        #endregion


        #region Properties

        private int Health
        {
            get => _health;
            set
            {
                _health = Mathf.Clamp(value, 0, MAX_HEALTH);
                if (_healthBar)
                    _healthBar.value = _health;
                if (_healthText)
                    _healthText.text = _health.ToString();
            }
        }

        #endregion


        #region UnityMethods

        void Start()
        {
            ResetHealth();
            _startHealingButton?.onClick.AddListener(ReceiveHealing);
            _resetHealthButton?.onClick.AddListener(ResetHealth);
        }

        private void OnDestroy()
        {
            _startHealingButton?.onClick.RemoveAllListeners();
            _resetHealthButton?.onClick.RemoveAllListeners();
        }

        #endregion


        #region Methods

        public void ReceiveHealing()
        {
            if (_healing != null && !_canRenewHealing)
                return;
                
            if (_healing != null)
                StopCoroutine(_healing);

            _healing = StartCoroutine(Healing());
        }

        public void ResetHealth()
        {
            Health = 1;
        }

        IEnumerator Healing()
        {
            var duration = 0f;

            while (duration < HEALING_DURATION && Health < MAX_HEALTH)
            {
                yield return new WaitForSeconds(HEALING_INTERVAL);
                duration += HEALING_INTERVAL;
                Health += HEALING_AMOUNT;
            }

            _healing = null;
        }

        #endregion
    }
}