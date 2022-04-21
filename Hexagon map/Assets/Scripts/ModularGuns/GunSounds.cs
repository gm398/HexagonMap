using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunSounds : MonoBehaviour
{


    [SerializeField] AudioSource[] shotSounds;
    [SerializeField] AudioSource[] OOAmmoSounds;
    [SerializeField] AudioSource[] reloadSounds;

    public void Shoot()
    {
        if (shotSounds.Length > 0)
        {
            shotSounds[Random.Range(0, shotSounds.Length)].Play();
        }
    }
    public void OutOfAmmo()
    {
        if (OOAmmoSounds.Length > 0)
        {
            OOAmmoSounds[Random.Range(0, OOAmmoSounds.Length)].Play();
        }
    }
    public void Reload()
    {
        if (reloadSounds.Length > 0)
        {
            reloadSounds[Random.Range(0, reloadSounds.Length)].Play();
        }
    }
}
