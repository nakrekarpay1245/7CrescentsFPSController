using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float damage = 20;
    public float range = 100;
    public Camera fpsCamera;

    public ParticleSystem muzzleFlash;
    //public GameObject impactEffectPrefab;

    public float fireRate;
    private float nextTimeToFire;

    public int currentBulletCount;
    public int magazineBulletCount;
    public int totalBulletCount;

    public Animator animator;
    public AnimationClip animation;


    bool isReload;
    private void Awake()
    {
        magazineBulletCount = currentBulletCount;
        WeaponUI.weaponUI.DisplayBulletCount(totalBulletCount, magazineBulletCount);
    }
    private void Update()
    {
        if (Input.GetKey(KeyCode.Mouse0))
        {
            WeaponUI.weaponUI.DisplayBulletCount(totalBulletCount, magazineBulletCount);

            if (Time.time > nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1 / fireRate;
                if (magazineBulletCount > 0)
                {
                    Shoot();
                }
                else if (magazineBulletCount <= 0)
                {
                    if (!isReload)
                    {
                        isReload = true;
                        Reload();
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.R) && magazineBulletCount < currentBulletCount)
        {
            if (!isReload)
            {
                isReload = true;
                Reload();
            }
        }
    }

    private void Reload()
    {

        if (totalBulletCount > 0)
        {
            animator.SetTrigger("isReload");
        }
        else
        {
            Debug.Log("Ammo bitti");
        }
    }

    public void SetBulletCount()
    {
        totalBulletCount += magazineBulletCount;
        if (totalBulletCount >= currentBulletCount)
        {
            magazineBulletCount = currentBulletCount;
            totalBulletCount -= currentBulletCount;
        }
        else if (totalBulletCount < currentBulletCount)
        {
            magazineBulletCount = totalBulletCount;
            totalBulletCount = 0;
        }
        WeaponUI.weaponUI.DisplayBulletCount(totalBulletCount, magazineBulletCount);
        isReload = false;
    }
    private void Shoot()
    {
        isReload = false;
        magazineBulletCount--;
        animator.SetTrigger("isShoot");
        muzzleFlash.Play();
        RaycastHit hit;
        if (Physics.Raycast(fpsCamera.transform.position, fpsCamera.transform.forward, out hit, range))
        {
            Target target = hit.transform.GetComponent<Target>();
            if (target != null)
            {
                target.TakeDamage(damage);
            }
            //GameObject impactEffect = Instantiate(impactEffectPrefab, hit.point, Quaternion.LookRotation(hit.normal));
            //Destroy(impactEffect, 2);
        }
    }
}
