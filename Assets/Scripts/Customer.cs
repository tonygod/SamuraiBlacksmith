using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Customer", menuName = "Customer", order = 2)]
public class Customer : ScriptableObject
{
    public Weapon weapon;
    public Sprite idleSprite;
    public Sprite joySprite;
    public Sprite sadSprite;
    public string needWeaponText;
    public string goodText;
    public string okText;
    public string badText;
    public Sprite scrollSprite;
}
