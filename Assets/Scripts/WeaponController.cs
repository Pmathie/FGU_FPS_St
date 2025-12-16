using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class WeaponController : MonoBehaviour
{
    public List<GameObject> weapons;
    private int currentWeaponIndex = 0;
    private float nextFireTime = 0;
    private Camera fpsCam;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        fpsCam = Camera.main;
        weapons[currentWeaponIndex].SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButton("Fire1")) 
        {
            Shoot();
        }
    }
    void Shoot()
    {
        if(Time.time < nextFireTime)
        {
            return;
        }

        Weapon weapon = weapons[currentWeaponIndex].GetComponent<Weapon>();
        nextFireTime = Time.time + weapon.fireRate;
        weapon.ShotShound.Play();
        weapon.animator.Play("DesertEagleShot", 0, 0f);

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out RaycastHit hit, 100f))
        {
            Debug.Log("Hit" + hit.transform.name);
        }
    }
}
