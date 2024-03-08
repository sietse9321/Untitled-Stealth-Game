using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    public float sensX = 10f;
    public float sensY = 10f;
    public float speed = 100f;
    [SerializeField] Transform parentTransform;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        float moveHorizontal = speed * Input.GetAxis("Mouse X") * sensX * Time.deltaTime;
        float moveVertical = -speed * Input.GetAxis("Mouse Y") * sensY * Time.deltaTime;
        moveVertical = Mathf.Clamp(moveVertical, -90f, 90f);
        parentTransform.transform.eulerAngles += new Vector3(0f, moveHorizontal, 0f);
        transform.eulerAngles += new Vector3(moveVertical, 0f, 0f);
    }
}
