using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintUI : MonoBehaviour
{
    public Camera cam;
    public Transform followingTarget;
    public Vector3 offset;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (cam == null || followingTarget == null) return;
        transform.position = cam.WorldToScreenPoint(followingTarget.position + offset);
    }

    public void SetCameraAndFollowingTarget(Camera targetCamera, Transform targetTransform)
    {
        this.cam = targetCamera;
        this.followingTarget = targetTransform;
    }
    public void SetOffset(Vector3 _offset)
    {
        offset = _offset;
    }
}
