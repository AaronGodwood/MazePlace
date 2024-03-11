using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Moving : MonoBehaviour
{
    private Vector3 movementInput;
    private Vector2 mouseInput;
    private float xRotation;

    public Transform PlayerCamera;
    public Rigidbody PlayerBody;
    public float speed;
    public float sensitivity;
    public float Jumpforce;
    // Start is called before the first frame update

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        movementInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        MovePlayer();
        MovePlayerCamera();


    }

    private void MovePlayer()
    {
        Vector3 movement = transform.TransformDirection(movementInput) * speed;
        PlayerBody.velocity = new Vector3(movement.x,PlayerBody.velocity.y,movement.z);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            PlayerBody.AddForce(Vector3.up * Jumpforce, ForceMode.Impulse);
        }

    }

    private void MovePlayerCamera()
    {
        xRotation -= mouseInput.y * sensitivity;
        transform.Rotate(0f, mouseInput.x * sensitivity, 0f);
        PlayerCamera.transform.localRotation = Quaternion.Euler(xRotation,0f,0f);
    }
}
