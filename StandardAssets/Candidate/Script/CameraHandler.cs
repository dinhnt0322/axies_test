using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{

    private float _sensitivity;
    private Vector3 _mouseReference;
    public Camera mainCamera;

    void Start()
    {
        _sensitivity = 0.05f;
    }
    void Update()
    {
        onHandlingCamera();
        if (Input.GetMouseButtonDown(0))
        {
            _mouseReference = Input.mousePosition;
            return;
        }
        if (!Input.GetMouseButton(0)) return;
            Vector3 pos = mainCamera.ScreenToViewportPoint(Input.mousePosition - _mouseReference);
            Vector3 move = new Vector3(pos.x * _sensitivity, pos.y * _sensitivity, 0);
            mainCamera.transform.Translate(move, Space.World);
    }
    void onHandlingCamera()
    {
        float zoomChangeAmount = 20f;
        if (Input.mouseScrollDelta.y > 0)
        {
            mainCamera.orthographicSize -= zoomChangeAmount * Time.deltaTime;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            mainCamera.orthographicSize += zoomChangeAmount * Time.deltaTime;
        }
    }
}
