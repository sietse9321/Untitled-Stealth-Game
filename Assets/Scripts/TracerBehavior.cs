using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class TracerBehavior : MonoBehaviour
{
    private float speed = 100f;
    private Vector3 targetPosition;

    public void SetTarget(Vector3 _target)
    {
        targetPosition = _target;
    }

    private void Update()
    {
        Vector3 direction = targetPosition - transform.position;
        transform.position += direction.normalized * speed * Time.deltaTime;

        if (Vector3.Dot(direction, targetPosition - transform.position) <= 0)
        {
            Destroy(gameObject);
        }
    }
}
