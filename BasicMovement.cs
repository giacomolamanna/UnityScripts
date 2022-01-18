using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script per il movimento in avanti e indietro di un 3D Object
//Creo un oggetto ad esempio un cubo, aggiungo questo script, 
//e trascino la MainCamera all'interno dell'oggetto
//Non è implementato il salto dell'oggetto

public class BasicMovement : MonoBehaviour
{
    float speed = 15;
    float rotationSpeed = 150;
    float rotation = 0f;
    float gravity = 9.81f;

    Vector3 moveDirection = Vector3.zero;

    CharacterController controller;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
    }




    void Movement()
    {
        if (controller.isGrounded)
        {
            if (Input.GetKey(KeyCode.UpArrow)) // Avanti
            {
                moveDirection = new Vector3(0, 0, 1);
                moveDirection *= speed;
                moveDirection = transform.TransformDirection(moveDirection);
            }
            else if (Input.GetKey(KeyCode.DownArrow)) // Indietro
            {
                moveDirection = new Vector3(0, 0, 1);
                moveDirection *= speed;
                moveDirection = transform.TransformDirection(-moveDirection);
            }
            else
            {
                moveDirection = new Vector3(0, 0, 0);
            }
        }

        rotation += Input.GetAxis("Horizontal") * rotationSpeed * Time.deltaTime;

        transform.eulerAngles = new Vector3(0, rotation, 0);

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);
    }
}









