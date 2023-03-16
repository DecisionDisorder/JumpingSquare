using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class Movement3D : MonoBehaviour
{
    public Rigidbody rigidbody;

    [SerializeField]
    private Vector3 moveDirection;
    [SerializeField]
    private float sprintSpeed = 15.0f;
    [SerializeField]
    private float walkSpeed = 10.0f;

    private float moveSpeed { get { return isSprint ? sprintSpeed : walkSpeed; } }
    [SerializeField]
    private float jumpMoveSpeed = 10.0f;
    [SerializeField]
    private float jumpForce = 3.0f;
    [SerializeField]
    private float climbingSpeed = 1.0f;

    [SerializeField]
    private Vector3 inputDirection;

    [SerializeField]
    private bool isSprint = false;
    [SerializeField]
    private bool isJumping = false;
    [SerializeField]
    private bool isClimbing = false;

    IEnumerator climbingCoroutine = null;

    void Awake()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
    }

    public void Move(Vector3 direction)
    {
        Move(direction, transform.position);
    }
    public void Move(Vector3 direction, Vector3 position)
    {
        if(!isClimbing)
        {
            Vector3 moveHorizontal = transform.right * direction.x;
            Vector3 moveVertical = transform.forward * direction.z;

            moveDirection = (moveHorizontal + moveVertical).normalized * moveSpeed;

            rigidbody.MovePosition(position + moveDirection * Time.deltaTime);
        }
        else
        {
            if (climbingCoroutine == null)
                StartCoroutine(climbingCoroutine = Climbing());
        }
    }

    public void SetInputDirection(Vector3 direction)
    {
        inputDirection = direction;
    }

    public void Jump()
    {
        if(!isJumping)
        {
            isJumping = true;
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void SprintOn()
    {
        isSprint = true;

    }

    public void SprintOff()
    {
        isSprint = false;
    }

    IEnumerator Climbing()
    {
        yield return new WaitForEndOfFrame();

        if(inputDirection.z == 1)
            rigidbody.MovePosition(transform.position + Vector3.up * climbingSpeed * Time.deltaTime);
        else
            rigidbody.MovePosition(transform.position - Vector3.up * climbingSpeed * Time.deltaTime);

        if (isClimbing) { StartCoroutine(Climbing()); }
        else { climbingCoroutine = null; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
        else if(collision.gameObject.CompareTag("Ladder"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = false;
            isClimbing = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ladder"))
        {
            rigidbody.useGravity = true;
            isClimbing = false;
        }
    }

    public void Rotation(float rotateX, float rotateY)
    {
        rigidbody.rotation = Quaternion.Euler(transform.rotation.x - rotateX, transform.rotation.y + rotateY, 0.0f);
    }
}
