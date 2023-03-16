using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space;
    [SerializeField]
    private KeyCode sprintKeyCode = KeyCode.LeftShift;

    public Camera playerCamera;
    public Rigidbody rigidbody;
    private Movement3D movement3D;

    [SerializeField]
    private float rotateSpeed = 5.0f;
    [SerializeField]
    private float limitAngle = 70.0f;

    private float mouseX;
    private float mouseY;
    
    public Vector3 moveDirection { private set; get; }

    public bool alive { private set; get; }
    public bool cleared { private set; get; }

    public PlayManager playManager;

    void Start()
    {
        alive = true;
        cleared = false;

        if(movement3D == null)
            movement3D = GetComponent<Movement3D>();

        mouseX = transform.rotation.eulerAngles.y;
        mouseY = -transform.rotation.eulerAngles.x;
    }

    void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        moveDirection = new Vector3(x, 0, z);
        movement3D.Move(moveDirection);

        movement3D.SetInputDirection(moveDirection);

        if(Input.GetKeyDown(jumpKeyCode))
        {
            movement3D.Jump();
        }
        if(Input.GetKeyDown(sprintKeyCode)) 
        {
            movement3D.SprintOn();
        }
        
        if(Input.GetKeyUp(sprintKeyCode))
        {
            movement3D.SprintOff();
        }
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            transform.position = Vector3.up;
        }
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.position = new Vector3(9, 2.8f, 11.5f);
        }


        MouseRotation();
    }

    private void MouseRotation()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
        mouseY = Mathf.Clamp(mouseY + Input.GetAxis("Mouse Y") * rotateSpeed, -limitAngle, limitAngle);

        movement3D.Rotation(0, mouseX);
        playerCamera.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.x - mouseY, transform.rotation.eulerAngles.y, 0);
    }

    public void Death()
    {
        if(alive)
        {
            alive = false;
            playManager.DeathUIEnable();
            rigidbody.useGravity= false;
            rigidbody.velocity = Vector3.zero;
            Debug.Log("Death!");
        }
    }

    public void Respawn(Vector3 respawnPosition)
    {
        if(!alive)
        {
            transform.position = respawnPosition;
            alive = true;
            rigidbody.useGravity = true;
            playManager.OnRespawn();
            Debug.Log("Respawn!");
        }
    }

    public void StageClear()
    {
        if(!cleared)
        {
            cleared = true;
            playManager.OnStageClear();
        }
    }
}
