using System.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GunBehavior", order = 1)]
public class GunBehavior : MonoBehaviour
{
    [SerializeField] string gunName;
    bool canFire = true;
    [SerializeField] GunType[] gunType;
    [SerializeField] GunType selectedGun;
    [SerializeField] Transform shootPos, adsPos, defaultPos;
    [SerializeField] Transform cam;
    float reloadTime, rateOfFire = 1f;
    [SerializeField] GameObject tracerPrefab;
    [SerializeField] bool aiming = false;


    [Header("UI")]
    [SerializeField] Camera mainCamera;
    [SerializeField] RectTransform dot;
    [SerializeField] float maxRaycastDistance = 100f;

    [Header("Sway/Bobbing Settings")]
    [SerializeField] float swayMulti;
    [SerializeField] float smooth;
    [SerializeField] float bobbingSpeed = 2f;
    [SerializeField] float bobbingAmount = 0.01f;

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
        if (Input.GetMouseButton(0) && canFire)
        {
            Shoot();
            StartCoroutine(ROF(rateOfFire));
        }
        GunPos();
        WeaponBobbing();
    }
    private void FixedUpdate()
    {
        WeaponSway();
        ShootDirToUI();
    }
    private void WeaponSwitch()
    {
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            
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
            shootPos.position = Vector3.Lerp(shootPos.position, adsPos.position, 5f * Time.deltaTime / Vector3.Distance(shootPos.position, adsPos.position));
            swayMulti = 2;
            aiming = true;
        }
        else
        {
            shootPos.position = Vector3.Lerp(shootPos.position, defaultPos.position, 5f * Time.deltaTime / Vector3.Distance(shootPos.position, defaultPos.position));
            swayMulti = 8;
            aiming = false;
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
        else if(newY != originalY)
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

        if (Physics.Raycast(ray, out hit, maxRaycastDistance))
        {
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(hit.point);
            dot.position = screenPoint;
        }
        else
        {
            Vector3 rayEnd = shootPos.position + gunDirection * maxRaycastDistance;
            Vector2 screenPoint = mainCamera.WorldToScreenPoint(rayEnd);
            dot.position = screenPoint;
        }
        Debug.DrawRay(ray.origin, ray.direction * maxRaycastDistance, Color.yellow);
    }
    private void SetBaseValues()
    {
        rateOfFire = selectedGun.rateOfFire;
        defaultPos.transform.position = selectedGun.defaultPos;
        adsPos.transform.position = selectedGun.adsPos;
    }

    private IEnumerator ROF(float time)
    {
        yield return new WaitForSeconds(time);
        canFire = true;
    }

}
