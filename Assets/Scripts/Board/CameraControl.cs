using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    [Header("Camera Movement")]
    [SerializeField] private float cameraMovementSpeed;
    [SerializeField] private float smoothCameraSpeed;
    [SerializeField] private float minCameraSpeed;
    [SerializeField] private float maxCameraSpeed;

    private float cameraMovementVelocityX;
    private float cameraMovementVelocityY;
    
    private float verticalInput;
    private float horizontalInput;

    [Header("Camera Zoom")]
    [SerializeField] private float zoomMultiplier;
    [SerializeField] private float smoothTime;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    private float zoomVelocity;
    private float zoom;

    [SerializeField] CinemachineVirtualCamera vCam;

    void Start()
    {
        zoom = vCam.m_Lens.OrthographicSize;
    }

    void Update()
    {
        if (UIManager.instance.isMouseOverMissionCardsScroller)
        {
            return;
        }

        if (Input.GetKey(KeyCode.Mouse1))
        {
            CalculateMovement();
        }

        if (verticalInput != 0 || horizontalInput != 0)
        {
            MoveCamera();
        }

        ZoomCamera();
    }

    private void ZoomCamera()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        zoom -= scroll * zoomMultiplier;
        zoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        vCam.m_Lens.OrthographicSize = Mathf.SmoothDamp(vCam.m_Lens.OrthographicSize, zoom, ref zoomVelocity, smoothTime);
    }

    private void MoveCamera()
    {
        verticalInput = Mathf.SmoothDamp(verticalInput, 0, ref cameraMovementVelocityX, smoothCameraSpeed);
        horizontalInput = Mathf.SmoothDamp(horizontalInput, 0, ref cameraMovementVelocityY, smoothCameraSpeed);

        transform.Translate(-horizontalInput, -verticalInput, 0, Space.World);

        if (Mathf.Approximately(cameraMovementVelocityX, 0.01f) && Mathf.Approximately(cameraMovementVelocityY, 0.01f))
        {
            verticalInput = 0;
            horizontalInput = 0;
        }
    }

    private void CalculateMovement()
    {
        if (Input.GetAxis("Mouse X") != 0 || Input.GetAxis("Mouse Y") != 0)
        {
            verticalInput = Input.GetAxis("Mouse Y") * cameraMovementSpeed * Time.deltaTime;
            horizontalInput = Input.GetAxis("Mouse X") * cameraMovementSpeed * Time.deltaTime;

            verticalInput = Mathf.Clamp(verticalInput, minCameraSpeed, maxCameraSpeed);
            horizontalInput = Mathf.Clamp(horizontalInput, minCameraSpeed, maxCameraSpeed);
        }
    }
}
