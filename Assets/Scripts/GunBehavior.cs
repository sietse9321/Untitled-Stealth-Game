using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GunBehavior : MonoBehaviour
{
    [SerializeField] string gunName;
    bool canFire = true;
    [SerializeField] Transform shootPos;
    [SerializeField] Transform cam;
    float reloadTime, ammo, sensitivity = 1f, rotationSpeed = 200f, moveHorizontal, moveVertical, maxDistance = 100f, rateOfFire = 1f;
    [SerializeField] GameObject tracerPrefab;
    


    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Update()
    {
        CameraMovement();

        if (Input.GetMouseButton(0) && canFire)
        {
            Shoot();
            StartCoroutine(ROF(rateOfFire));
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
            Debug.DrawRay(shootPos.position,shootPos.forward* 100f, Color.green, 1f);

            print("Hit something: " + hit.point);
            GameObject tracer = Instantiate(tracerPrefab, shootPos.position, Quaternion.identity);
            tracer.GetComponent<TracerBehavior>().SetTarget(hit.point);
            canFire = false;
        }
        else
        {
            Debug.DrawRay(shootPos.position, shootPos.forward*100f, Color.red, 1f);

            print("No hit");
            Vector3 tracerPos = shootPos.position + shootPos.forward * 100f;
            GameObject tracer = Instantiate(tracerPrefab, shootPos.position, Quaternion.identity);
            tracer.GetComponent<TracerBehavior>().SetTarget(tracerPos);
            canFire = false;
        }
    }


    private void CameraMovement()
    {
        float moveHorizontal = rotationSpeed * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float moveVertical = -rotationSpeed * Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;
        moveVertical = Mathf.Clamp(moveVertical, -90f, 90f);
        transform.eulerAngles += new Vector3(moveVertical, moveHorizontal, 0f);
    }


    private IEnumerator ROF(float time)
    {
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
