using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLook : MonoBehaviour
{
    [Header("Look Parameters")]
    [SerializeField]
    private float sensitivityX = 2;

    [SerializeField]
    private float sensitivityY = 2;

    [SerializeField]
    private float mouseX = 2;

    [SerializeField]
    private float mouseY = 2;

    [SerializeField]
    private float multiplier = 0.01f;

    [SerializeField]
    private float xRotation = 0;

    [SerializeField]
    private float yRotation = 0;

    [SerializeField]
    private Transform playerCamera;

    [SerializeField]
    private Transform orientation;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void Update()
    {
        MyInput();
        playerCamera.transform.localRotation = Quaternion.EulerAngles(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.EulerAngles(0, yRotation, 0);
    }

    private void MyInput()
    {
        mouseX = Input.GetAxisRaw("Mouse X");
        mouseY = Input.GetAxisRaw("Mouse Y");

        yRotation += mouseX * sensitivityX * multiplier;
        xRotation -= mouseY * sensitivityY * multiplier;

        xRotation = Mathf.Clamp(xRotation, -80, 80);
        yRotation = Mathf.Clamp(yRotation, -80, 80);
    }
}
