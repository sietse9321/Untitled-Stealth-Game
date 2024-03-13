using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class GunBehavior : MonoBehaviour
{
    public bool isUnarmed;
    [SerializeField] string gunName;

    [SerializeField] GunType[] gunType;
    [SerializeField] GameObject[] gunObjects;
    [SerializeField] GunType selectedGun;
    [SerializeField] Transform shootPos, adsPos, defaultPos;
    [SerializeField] Transform cam;
    float reloadTime, rateOfFire = 0.2f; //? 0,20 for pistol
    bool isAutomatic = false;
    [SerializeField] GameObject tracerPrefab;
    bool aiming = false;
    bool isShooting;
    int ammo;

    [SerializeField] GameObject scope;

    [Header("Sway/Bobbing Settings")]
    [SerializeField] float swayMulti;
    [SerializeField] float smooth;
    [SerializeField] float bobbingSpeed = 2f;
    [SerializeField] float bobbingAmount = 0.01f;

    [Header("UI")]
    [SerializeField] TextMeshProUGUI ammoText;
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform dot;
    [SerializeField] Image img;

    ObjectPool<TracerBehavior> tracerPool;

    //int pistolAmmo = 111, rifleAmmo = 230;
    bool canFire = true;
    float newY;
    float originalY;



    // Start is called before the first frame update
    private void Start()
    {
        isUnarmed = true;
        Cursor.lockState = CursorLockMode.Locked;
        originalY = shootPos.localPosition.y;
        img.enabled = false;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isAutomatic && canFire && ammo > 0)
        {
            Shoot();
            StartCoroutine(ROF(rateOfFire));
        }

        if (Input.GetMouseButton(0) && isAutomatic == false && canFire && isShooting == false && ammo > 0)
        {
            Shoot();
            isShooting = true;
            StartCoroutine(ROF(rateOfFire));
        }
        else if (Input.GetMouseButtonUp(0))
        {
            isShooting = false;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            StartCoroutine(Reload(reloadTime));
        }
        GunPos();
        WeaponBobbing();
        WeaponSwitch();
    }
    private void FixedUpdate()
    {
        WeaponSway();
        ShootDirToUI();
        CanvasUpdate();
    }
    private void CanvasUpdate()
    {
        if (!isUnarmed)
        {
            ammoText.text = ammo + "/" + selectedGun.magCap;
        }
        else
        {
            ammoText.text = "Unarmed";
        }
    }

    #region Switching gun
    private void WeaponSwitch()
    {

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SwitchToUnarmed();
            StopAllCoroutines();
            return;
        }

        for (int i = 0; i < gunObjects.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                SwitchToGun(i);
                StopAllCoroutines();
                break;
            }
        }
    }
    private void SwitchToGun(int index)
    {
        if (index < 0 || index >= gunObjects.Length)
            return;

        foreach (GameObject gun in gunObjects)
        {
            gun.SetActive(false);
        }
        gunObjects[index].SetActive(true);
        selectedGun = gunType[index];
        SetBaseValues();
        scope.SetActive(true);
        isUnarmed = false;
        canFire = true;
        img.enabled = true;
    }
    private void SwitchToUnarmed()
    {
        foreach (GameObject gun in gunObjects)
        {
            gun.SetActive(false);
        }
        img.enabled = false;
        isUnarmed = true;
        selectedGun = null;
        scope.SetActive(false);
        canFire = false;
    }
    #endregion

    private void Shoot()
    {
        if (shootPos == null)
        {
            Debug.LogError("Shoot position is not assigned!");
            return;
        }

        RaycastHit hit;
        if (Physics.Raycast(shootPos.position, shootPos.forward, out hit))
        {
            Debug.DrawRay(shootPos.position, shootPos.forward * 100f, Color.green, 1f);

            print("Hit something: " + hit.point);
            GameObject tracer = Instantiate(tracerPrefab, shootPos.position, shootPos.rotation);
            tracer.GetComponent<TracerBehavior>().SetTarget(hit.point);
            canFire = false;
        }
        else
        {
            Debug.DrawRay(shootPos.position, shootPos.forward * 100f, Color.red, 1f);

            print("No hit");
            Vector3 tracerPos = shootPos.position + shootPos.forward * 100f;
            GameObject tracer = Instantiate(tracerPrefab, shootPos.position, shootPos.rotation);
            tracer.GetComponent<TracerBehavior>().SetTarget(tracerPos);
            canFire = false;
        }
    }
    private void GunPos()
    {
        if (isUnarmed) { return; }
        //check if right mousebutten is pressed
        if (Input.GetMouseButton(1))
        {
            shootPos.position = Vector3.Lerp(shootPos.position, adsPos.position, 4f * Time.deltaTime / Vector3.Distance(shootPos.position, adsPos.position));
            swayMulti = 0.5f;
            aiming = true;
            scope?.SetActive(true);
            img.enabled = false;
        }
        else
        {
            shootPos.position = Vector3.Lerp(shootPos.position, defaultPos.position, 4f * Time.deltaTime / Vector3.Distance(shootPos.position, defaultPos.position));
            swayMulti = 8f;
            aiming = false;
            scope?.SetActive(false);
            img.enabled = true;
        }
    }
    private void WeaponSway()
    {
        float mouseX = Input.GetAxisRaw("Mouse X") * swayMulti;
        float mouseY = Input.GetAxisRaw("Mouse Y") * swayMulti;

        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        shootPos.localRotation = Quaternion.Slerp(shootPos.localRotation, targetRotation, smooth * Time.deltaTime);
    }
    private void WeaponBobbing()
    {
        if (aiming == false)
        {
            // Calculate the vertical bobbing motion using sine function
            newY = originalY + Mathf.Sin(Time.time * bobbingSpeed) * bobbingAmount;

            // Apply the bobbing motion to the weapon's local position
            shootPos.localPosition = new Vector3(shootPos.localPosition.x, newY, shootPos.localPosition.z);
        }
        else if (newY != originalY)
        {
            newY = originalY;
        }
    }
    private void ShootDirToUI()
    {


        //? rework code

        Vector3 gunDirection = shootPos.forward;

        Ray ray = new Ray(shootPos.position, gunDirection);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 50f))
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(hit.point);
            dot.position = screenPoint;
        }
        else
        {
            Vector3 rayEnd = shootPos.position + gunDirection * 50f;
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(rayEnd);
            dot.position = screenPoint;
        }
        Debug.DrawRay(ray.origin, ray.direction * 50f, Color.yellow);
    }
    private void SetBaseValues()
    {
        gunName = selectedGun.gunName;
        rateOfFire = selectedGun.rateOfFire;
        defaultPos.localPosition = selectedGun.defaultPos;
        adsPos.localPosition = selectedGun.adsPos;
        reloadTime = selectedGun.reloadTime;
        isAutomatic = selectedGun.isAutomatic;
        ammo = selectedGun.currentAmmo;
    }

    private IEnumerator Reload(float _time)
    {
        yield return new WaitForSeconds(_time);
        if (ammo >= 1)
        {
            ammo = selectedGun.magCap;
        }
        else
        {
            ammo = selectedGun.magCap - 1;
        }
        canFire = true;
        selectedGun.currentAmmo = ammo;
    }
    private IEnumerator ROF(float _time)
    {
        yield return new WaitForSeconds(_time);
        print("Shooting!");
        ammo--;
        selectedGun.currentAmmo--;
        canFire = true;
    }
}