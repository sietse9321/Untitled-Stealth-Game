using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TracerBehavior : MonoBehaviour
{
    private float speed = 100f;
    private Vector3 targetPosition;
    public GameObject cube;

    public void SetTarget(Vector3 _target)
    {
        targetPosition = _target;
    }

    private void Update()
    {
        //Vector3 direction = targetPosition - transform.position;
        transform.position += transform.forward * speed * Time.deltaTime;
        Destroy(gameObject, 1.5f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag != "Player")
        {
            print(collision);
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }
}
