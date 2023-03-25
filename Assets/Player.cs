using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// �÷��̾� ���� Ŭ����
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// ���� Ű �ڵ�
    /// </summary>
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space;
    /// <summary>
    /// �޸��� Ű �ڵ�
    /// </summary>
    [SerializeField]
    private KeyCode sprintKeyCode = KeyCode.LeftShift;

    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    public Camera playerCamera;
    /// <summary>
    /// �÷��̾��� ������ ó���� ���� Rigidbody
    /// </summary>
    public Rigidbody rigidbody;
    /// <summary>
    /// �÷��̾� �̵� ���� ������Ʈ
    /// </summary>
    private Movement3D movement3D;

    /// <summary>
    /// ���콺 ȸ�� �ӵ�
    /// </summary>
    [SerializeField]
    private float rotateSpeed = 5.0f;
    /// <summary>
    /// ���� ȸ�� ���� ����
    /// </summary>
    [SerializeField]
    private float limitAngle = 70.0f;

    /// <summary>
    /// ���콺 ȸ���� x
    /// </summary>
    private float mouseX;
    /// <summary>
    /// ���콺 ȸ���� y
    /// </summary>
    private float mouseY;
    
    /// <summary>
    /// �̵� ����
    /// </summary>
    public Vector3 moveDirection { private set; get; }

    /// <summary>
    /// ���� ����
    /// </summary>
    public bool alive { private set; get; }
    /// <summary>
    /// Ŭ���� ����
    /// </summary>
    public bool cleared { private set; get; }
    /// <summary>
    /// ���� ��� ó�� �Ұ� ����
    /// </summary>
    private bool deathCoolTime = false;

    /// <summary>
    /// ���� �÷��� �ý����� �����ϴ� ������Ʈ
    /// </summary>
    public PlayManager playManager;

    /// <summary>
    /// �÷��̾� ���� �� �ʱ�ȭ
    /// </summary>
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
        // ����Ű �Է°� �ޱ�
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // �̵� ó��
        moveDirection = new Vector3(x, 0, z);
        movement3D.Move(moveDirection);

        // �Է� �� ����
        movement3D.SetInputDirection(moveDirection);

        // ���� ó��
        if(Input.GetKeyDown(jumpKeyCode))
        {
            movement3D.Jump();
        }
        // �޸��� ó��
        if(Input.GetKeyDown(sprintKeyCode)) 
        {
            movement3D.SprintOn();
        }
        // �޸��� ���� ó��
        if(Input.GetKeyUp(sprintKeyCode))
        {
            movement3D.SprintOff();
        }
        // (�׽�Ʈ)��ġ �ʱ�ȭ
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            transform.position = Vector3.up;
        }
        // (�׽�Ʈ)��ٸ� ������ �̵�
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.position = new Vector3(9, 2.8f, 11.5f);
        }

        // ���콺 ȸ�� ����
        MouseRotation();
    }

    /// <summary>
    /// ���콺 ȸ������ �÷��̾� ȸ�� ����
    /// </summary>
    private void MouseRotation()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
        mouseY = Mathf.Clamp(mouseY + Input.GetAxis("Mouse Y") * rotateSpeed, -limitAngle, limitAngle);

        movement3D.Rotation(0, mouseX);
        playerCamera.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.x - mouseY, transform.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// �÷��̾� ��� ó��
    /// </summary>
    public void Death()
    {
        // ���� ��� ó�� ����
        if(!deathCoolTime)
        {
            // ��� UI ������Ʈ �� �߰� �̵����� �ʵ��� ó��
            playManager.DeathUIEnable();
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            Debug.Log("Death!");
        }
    }

    /// <summary>
    /// �÷��̾� ������ ó��
    /// </summary>
    public void Respawn()
    {
        // ������ �ƴ� ������ ����
        if(!alive)
        {
            // �������� ��ȯ �� ���� �� ���� ����
            alive = true;
            rigidbody.useGravity = true;
            // �ý��ۿ� ������ �˸�
            playManager.OnRespawn();
            Debug.Log("Respawn!");
            // ���� ��� ���� ó�� ��Ÿ�� ����
            StartCoroutine(DeathCoolTime());
        }
    }

    /// <summary>
    /// ���� ��� ó�� ���� �ڷ�ƾ
    /// </summary>
    IEnumerator DeathCoolTime()
    {
        deathCoolTime = true;
        yield return new WaitForSecondsRealtime(0.5f);

        deathCoolTime = false;
    }

    /// <summary>
    /// �������� Ŭ���� ó��
    /// </summary>
    public void StageClear()
    {
        if(!cleared)
        {
            cleared = true;
            playManager.OnStageClear();
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    /// <param name="alive">���� ����</param>
    public void SetAlive(bool alive)
    {
        this.alive = alive;
    }
}
