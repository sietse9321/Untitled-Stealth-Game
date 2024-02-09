using System;
using System.Collections;
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
    [SerializeField] ParticleSystem bulletParticle;
    [SerializeField] LineRenderer bulletTrail;


    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        bulletTrail.enabled = true;
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
       
    }


    private void CameraMovement()
    {
        float moveHorizontal = rotationSpeed * Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float moveVertical = -rotationSpeed * Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime; // Invert the vertical movement
        moveVertical = Mathf.Clamp(moveVertical, -90f, 90f);
        transform.eulerAngles += new Vector3(moveVertical, moveHorizontal, 0f);
    }


    private IEnumerator ROF(float time)
    {
        yield return new WaitForSeconds(time);
        canFire = true;
    }
}
