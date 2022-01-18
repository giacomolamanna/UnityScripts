using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // VARIABLES
    [SerializeField] GameObject player;
    [SerializeField] private float mouseSensitivity_X;
    [SerializeField] private float mouseSensitivity_Y;
    [SerializeField] float rotation = 0f;

    public float vistaSu = -15f;
    public float vistaGiu = 30f;

    void Start()
    {
        //nascondo e blocco il puntatore mentre gioco
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    
    void Update()
    {
        rotation += Input.GetAxis("Mouse X") * mouseSensitivity_X * Time.deltaTime;

        player.transform.eulerAngles = new Vector3(0, rotation, 0);

        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity_Y * Time.deltaTime;

        //transform.Rotate(Vector3.right, -mouseY);
        


        // Get the current rotation angles.
        Vector3 eulerAngles = transform.localEulerAngles;

        // Returned angles are in the range 0...360. Map that back to -180...180 for convenience.
        if (eulerAngles.x > 180f)
            eulerAngles.x -= 360f;

        // Increment the pitch angle, respecting the clamped range.
        eulerAngles.x = Mathf.Clamp(eulerAngles.x - mouseY, vistaSu, vistaGiu);

        // Orient to match the new angles.
        transform.localEulerAngles = eulerAngles;
    }
}


