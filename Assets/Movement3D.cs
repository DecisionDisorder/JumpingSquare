using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// �÷��̾� �̵� �ý��� ���� Ŭ����
/// </summary>
public class Movement3D : MonoBehaviour
{
    /// <summary>
    /// ���� ó�� Rigidbody
    /// </summary>
    public Rigidbody rigidbody;

    /// <summary>
    /// �̵� ����
    /// </summary>
    [SerializeField]
    private Vector3 moveDirection;
    /// <summary>
    /// �޸��� �ӵ�
    /// </summary>
    [SerializeField]
    private float sprintSpeed = 15.0f;
    /// <summary>
    /// �ȱ� �ӵ�
    /// </summary>
    [SerializeField]
    private float walkSpeed = 10.0f;

    /// <summary>
    /// �̵� �ӵ� (�޸���/�ȱ� �߿� ���¿� ���߾� ��ȯ)
    /// </summary>
    private float moveSpeed { get { return isSprint ? sprintSpeed : walkSpeed; } }
    /// <summary>
    /// ������ �̵��ӵ�
    /// </summary>
    [SerializeField]
    private float jumpMoveSpeed = 10.0f;
    /// <summary>
    /// ���� Force
    /// </summary>
    [SerializeField]
    private float jumpForce = 3.0f;
    /// <summary>
    /// ��ٸ� ������ �ӵ�
    /// </summary>
    [SerializeField]
    private float climbingSpeed = 1.0f;

    /// <summary>
    /// ������� �Է� ����
    /// </summary>
    [SerializeField]
    private Vector3 inputDirection;

    /// <summary>
    /// �޸��� ������ ����
    /// </summary>
    [SerializeField]
    private bool isSprint = false;
    /// <summary>
    /// �����ϴ� ������ ����
    /// </summary>
    [SerializeField]
    private bool isJumping = false;
    /// <summary>
    /// ��ٸ��� Ÿ�� ������ ����
    /// </summary>
    [SerializeField]
    private bool isClimbing = false;

    /// <summary>
    /// ��ٸ� Ÿ�� �ڷ�ƾ ĳ�� ������Ʈ
    /// </summary>
    IEnumerator climbingCoroutine = null;

    void Awake()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// �̵� ���⿡ ���� �̵� ó��
    /// </summary>
    /// <param name="direction">�̵� ����</param>
    public void Move(Vector3 direction)
    {
        Move(direction, transform.position);
    }
    /// <summary>
    /// ���� ������ �̵� ó��
    /// </summary>
    /// <param name="direction">�̵� ����</param>
    /// <param name="position">������</param>
    public void Move(Vector3 direction, Vector3 position)
    {
        // ��ٸ� ��� ���� �ƴϸ�
        if(!isClimbing)
        {
            // ����/���� ���� �Է°��� ���� ȸ������ ���� �� ����
            Vector3 moveHorizontal = transform.right * direction.x;
            Vector3 moveVertical = transform.forward * direction.z;

            // �Է°��� �ϳ��� ���ͷ� ��ġ�� �̵� �ӵ����� �ջ��Ͽ� �̵� ���� ���
            moveDirection = (moveHorizontal + moveVertical).normalized * moveSpeed;

            // �̵� ����
            rigidbody.MovePosition(position + moveDirection * Time.deltaTime);
        }
        else
        {
            if (climbingCoroutine == null)
                StartCoroutine(climbingCoroutine = Climbing());
        }
    }

    /// <summary>
    /// �Է� ���� ����
    /// </summary>
    /// <param name="direction">�Է� ����</param>
    public void SetInputDirection(Vector3 direction)
    {
        inputDirection = direction;
    }

    /// <summary>
    /// �÷��̾� ����
    /// </summary>
    public void Jump()
    {
        // ���� ���� �ƴ� ������ ���
        if(!isJumping)
        {
            // ���� ���·� ���� �� ������������ �������� ���� ���Ѵ�
            isJumping = true;
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// �޸��� On
    /// </summary>
    public void SprintOn()
    {
        isSprint = true;

    }

    /// <summary>
    /// �޸��� Off
    /// </summary>
    public void SprintOff()
    {
        isSprint = false;
    }

    /// <summary>
    /// ��ٸ� ��� ó�� �ڷ�ƾ
    /// </summary>
    IEnumerator Climbing()
    {
        yield return new WaitForEndOfFrame();

        // ������ ���� ��ư�� ������ ���� �� ���
        if(inputDirection.z == 1)
            rigidbody.MovePosition(transform.position + Vector3.up * climbingSpeed * Time.deltaTime);
        // ������ �Ȱ����� �ϰ�
        else
            rigidbody.MovePosition(transform.position - Vector3.up * climbingSpeed * Time.deltaTime);

        if (isClimbing) { StartCoroutine(Climbing()); }
        else { climbingCoroutine = null; }
    }

    /// <summary>
    /// �÷��̾� �浹 ó��
    /// </summary>
    /// <param name="collision">�浹ü</param>
    private void OnCollisionEnter(Collision collision)
    {
        // ���� �浹�ϸ� ���� ���� ó��
        if(collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
        // ��ٸ��� �浹�ϸ� ��� ó�� �� �׿� �ʿ��� �� ����
        else if(collision.gameObject.CompareTag("Ladder"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = false;
            isClimbing = true;
        }
    }

    /// <summary>
    /// �÷��̾� �浹�� ���� ���� ó��
    /// </summary>
    /// <param name="collision">�浹ü</param>
    private void OnCollisionExit(Collision collision)
    {
        // ��ٸ����� ��������, ��� ��� ��� �� �� ���� ����
        if (collision.gameObject.CompareTag("Ladder"))
        {
            rigidbody.useGravity = true;
            isClimbing = false;
        }
    }

    /// <summary>
    /// �÷��̾� ȸ�� ����
    /// </summary>
    /// <param name="rotateX">x�� ȸ�� ��</param>
    /// <param name="rotateY">y�� ȸ�� ��</param>
    public void Rotation(float rotateX, float rotateY)
    {
        rigidbody.rotation = Quaternion.Euler(transform.rotation.x - rotateX, transform.rotation.y + rotateY, 0.0f);
    }
}
