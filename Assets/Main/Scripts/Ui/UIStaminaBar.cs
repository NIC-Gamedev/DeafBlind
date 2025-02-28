using UnityEngine;
using UnityEngine.UI;

namespace Main.Scripts.Ui
{
    public class UIStaminaBar : MonoBehaviour
    {
        [SerializeField] private PlayerNetworkMovement playerNetwork;
        private Slider _slider;

        void Start()
        {
            _slider = GetComponent<Slider>();
            _slider.maxValue = playerNetwork.maxStaminaTime;
            playerNetwork.OnStaminaChanged += OnStaninaChange;
        }

        void OnDestroy()
        {
            playerNetwork.OnStaminaChanged -= OnStaninaChange;
        }

        private void OnStaninaChange(float currentHealth)
        {
            _slider.value = currentHealth;
        }
    }
}
