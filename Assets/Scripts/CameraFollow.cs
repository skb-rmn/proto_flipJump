using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform targetTransform;
    [SerializeField] Vector3 offset;
    private void LateUpdate()
    {
        transform.position = targetTransform.position + offset;
        transform.LookAt(targetTransform);
    }
}
