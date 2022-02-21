using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponUI : MonoBehaviour
{
    public GameObject crosshair;

    public Text totalBulletCountText;
    public Text magazineBulletCountText;

    public static WeaponUI weaponUI;

    private void Awake()
    {
        if (weaponUI == null)
        {
            weaponUI = this;
        }
    }

    public void DisplayBulletCount(int totalBullet, int magazineBullet)
    {
        totalBulletCountText.text = totalBullet.ToString();
        magazineBulletCountText.text = magazineBullet.ToString();
    }
}
