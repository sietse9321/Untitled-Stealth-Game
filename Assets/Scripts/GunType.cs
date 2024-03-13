using UnityEngine;

[CreateAssetMenu(fileName = "Gun",menuName ="Weapon/Gun")]
public class GunType : ScriptableObject
{
    [Header("Gun Name")]
    public string gunName;

    [Header("Gun Propertys:")]
    public int magCap;
    public int currentAmmo;
    public float rateOfFire;
    public float reloadTime;
    public bool isAutomatic;

    [Header("Gun Positions:")]
    public Vector3 defaultPos;
    public Vector3 adsPos;
}