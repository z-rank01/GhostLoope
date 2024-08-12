using System.Collections;
using System.Collections.Generic;
using System.Reflection;
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
        if (followingTarget != null && cam != null)
        {
            transform.position = cam.WorldToScreenPoint(followingTarget.position + offset);
        }
        //Debug.Log("flolowingTarget: " + followingTarget.position);
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
