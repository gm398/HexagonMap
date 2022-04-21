using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunAnimationController : MonoBehaviour
{
    [SerializeField] Animator animator;
    GunController controller;

    private void Awake()
    {
        if(this.GetComponent<GunController>() != null)
        {
            controller = this.GetComponent<GunController>();
        }
    }

    void Shoot()
    {
        animator.speed = controller.GetAttacksPerSecond();
        animator.SetTrigger("Shoot");
    }
    void Reload()
    {
        animator.speed = controller.GetReloadTime();
        animator.SetTrigger("Reload");
    }
    void OutOfAmmo()
    {
        animator.speed = 1;
        animator.SetTrigger("OutOfAmmo");
    }
}
