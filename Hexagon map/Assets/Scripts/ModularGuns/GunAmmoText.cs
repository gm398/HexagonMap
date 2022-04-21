using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunAmmoText : MonoBehaviour
{


    [SerializeField] Text ammoCounter;
    GunController controller;

    private void Awake()
    {
        if (transform.GetComponent<GunController>() != null)
        {
            controller = transform.GetComponent<GunController>();
            Invoke("SetAmmoCounterText", .1f);
        }
    }
    private void OnEnable()
    {
        SetAmmoCounterText();
    }

    void Shoot()
    {
        SetAmmoCounterText();
    }

    void Reload()
    {
        ammoCounter.text = "Reloading";
        Invoke("SetAmmoCounterText", controller.GetReloadTime());

    }

    void SetAmmoCounterText()
    {
        if (ammoCounter != null)
        {
            float maxAmmo = controller.GetMaxAmmo();
            float currentAmmo = controller.GetCurrentAmmo();

            ammoCounter.text = currentAmmo + "/" + maxAmmo;

        }
    }
}
