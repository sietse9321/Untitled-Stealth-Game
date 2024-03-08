using Microsoft.Unity.VisualStudio.Editor;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GunBehavior : MonoBehaviour
{
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
    float ammo;

    [SerializeField] GameObject scope;

    [Header("Sway/Bobbing Settings")]
    [SerializeField] float swayMulti;
    [SerializeField] float smooth;
    [SerializeField] float bobbingSpeed = 2f;
    [SerializeField] float bobbingAmount = 0.01f;

    [Header("UI")]
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform dot;
    [SerializeField] float maxRaycastDistance = 100f;

    bool canFire = true;
    float newY;
    float originalY;



    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        originalY = shootPos.localPosition.y;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0) && isAutomatic && canFire)
        {
            Shoot();
            StartCoroutine(ROF(rateOfFire));
        }

        if (Input.GetMouseButton(0) && isAutomatic == false && canFire && isShooting == false)
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
            StartCoroutine(ROF(reloadTime));
        }
        GunPos();
        WeaponBobbing();
        WeaponSwitch();
    }
    private void FixedUpdate()
    {
        WeaponSway();
        ShootDirToUI();
    }
    
    private void WeaponSwitch()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            foreach (GameObject gun in gunObjects)
            {
                gun.SetActive(false);
            }
            gunObjects[0].SetActive(true);
            selectedGun = gunType[0];
            SetBaseValues();
            scope.SetActive(true);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            foreach (GameObject gun in gunObjects)
            {
                gun.SetActive(false);
            }
            gunObjects[1].SetActive(true);
            selectedGun = gunType[1];
            SetBaseValues();
            scope.SetActive(true);
        }
    }

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
        //check if right mousebutten is pressed
        if (Input.GetMouseButton(1))
        {
            shootPos.position = Vector3.Lerp(shootPos.position, adsPos.position, 4f * Time.deltaTime / Vector3.Distance(shootPos.position, adsPos.position));
            swayMulti = 2;
            aiming = true;
            scope?.SetActive(true);
        }
        else
        {
            shootPos.position = Vector3.Lerp(shootPos.position, defaultPos.position, 4f * Time.deltaTime / Vector3.Distance(shootPos.position, defaultPos.position));
            swayMulti = 8;
            aiming = false;
            scope?.SetActive(false);
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

        if (Physics.Raycast(ray, out hit, 10f))
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(hit.point);
            dot.position = screenPoint;
        }
        else
        {
            Vector3 rayEnd = shootPos.position + gunDirection * 10f;
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(rayEnd);
            dot.position = screenPoint;
        }
        Debug.DrawRay(ray.origin, ray.direction * 10f, Color.yellow);
    }
    private void SetBaseValues()
    {
        gunName = selectedGun.gunName;
        rateOfFire = selectedGun.rateOfFire;
        defaultPos.localPosition = selectedGun.defaultPos;
        adsPos.localPosition = selectedGun.adsPos;
        reloadTime = selectedGun.reloadTime;
        isAutomatic = selectedGun.isAutomatic;
        //wait(6)
        //bool = true
    }

    private IEnumerator Reload(float _time)
    {
        yield return new WaitForSeconds(_time);
        if(ammo > 1)
        {
            ammo = selectedGun.magCap + 1;
        }
        else
        {
            ammo = selectedGun.magCap;
        }
        canFire = true;
    }
    private IEnumerator ROF(float _time)
    {
        yield return new WaitForSeconds(_time);
        canFire = true;
    }
}