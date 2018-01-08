using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour {
    public Weapon[] weapons;

    public Weapon SelectRandomWeapon()
    {
        int i = Random.Range(0, weapons.Length);
        Debug.Log("Selected weapon: " + weapons[i].name);
        return weapons[i];
    }
}
