using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    private Vector3 offset;
    private Transform playerTransform;
    public float zoomSpeed = 5;
    // Start is called before the first frame update
    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = transform.position - playerTransform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = playerTransform.position + offset;

        float Scroll = Input.GetAxis("Mouse ScrollWheel");

        Camera.main.fieldOfView = Mathf.Clamp(Camera.main.fieldOfView + Scroll * zoomSpeed, 30, 80);

    }
}
