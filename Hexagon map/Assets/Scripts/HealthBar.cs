using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{


    [SerializeField] Slider slider;
    [SerializeField] Gradient gradient;
    [SerializeField] Image fill;
    [SerializeField] Health health;

    private void Awake()
    {
        if(this.gameObject.GetComponentInParent<Health>() != null)
        {
            health = this.gameObject.GetComponentInParent<Health>();
            SetMaxHealth(health.GetMaxHealth());
        }
    }

    public void SetMaxHealth(float health)
    {
        slider.maxValue = health;
        SetHealth(health);
        fill.color = gradient.Evaluate(1f);
    }
    public void DamageTaken()
    {
        SetHealth(health.GetCurrentHealth());
    }
    public void SetHealth(float health)
    {
        slider.value = health;
        fill.color = gradient.Evaluate(slider.normalizedValue);
    }
}
