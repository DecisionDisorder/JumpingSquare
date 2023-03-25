using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

/// <summary>
/// 플레이어 제어 클래스
/// </summary>
public class Player : MonoBehaviour
{
    /// <summary>
    /// 점프 키 코드
    /// </summary>
    [SerializeField]
    private KeyCode jumpKeyCode = KeyCode.Space;
    /// <summary>
    /// 달리기 키 코드
    /// </summary>
    [SerializeField]
    private KeyCode sprintKeyCode = KeyCode.LeftShift;

    /// <summary>
    /// 메인 카메라
    /// </summary>
    public Camera playerCamera;
    /// <summary>
    /// 플레이어의 물리적 처리를 위한 Rigidbody
    /// </summary>
    public Rigidbody rigidbody;
    /// <summary>
    /// 플레이어 이동 관리 오브젝트
    /// </summary>
    private Movement3D movement3D;

    /// <summary>
    /// 마우스 회전 속도
    /// </summary>
    [SerializeField]
    private float rotateSpeed = 5.0f;
    /// <summary>
    /// 상하 회전 제한 각도
    /// </summary>
    [SerializeField]
    private float limitAngle = 70.0f;

    /// <summary>
    /// 마우스 회전값 x
    /// </summary>
    private float mouseX;
    /// <summary>
    /// 마우스 회전값 y
    /// </summary>
    private float mouseY;
    
    /// <summary>
    /// 이동 방향
    /// </summary>
    public Vector3 moveDirection { private set; get; }

    /// <summary>
    /// 생존 여부
    /// </summary>
    public bool alive { private set; get; }
    /// <summary>
    /// 클리어 여부
    /// </summary>
    public bool cleared { private set; get; }
    /// <summary>
    /// 연속 사망 처리 불가 여부
    /// </summary>
    private bool deathCoolTime = false;

    /// <summary>
    /// 게임 플레이 시스템을 관리하는 오브젝트
    /// </summary>
    public PlayManager playManager;

    /// <summary>
    /// 플레이어 관련 값 초기화
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
        // 방향키 입력값 받기
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // 이동 처리
        moveDirection = new Vector3(x, 0, z);
        movement3D.Move(moveDirection);

        // 입력 값 전달
        movement3D.SetInputDirection(moveDirection);

        // 점프 처리
        if(Input.GetKeyDown(jumpKeyCode))
        {
            movement3D.Jump();
        }
        // 달리기 처리
        if(Input.GetKeyDown(sprintKeyCode)) 
        {
            movement3D.SprintOn();
        }
        // 달리기 종료 처리
        if(Input.GetKeyUp(sprintKeyCode))
        {
            movement3D.SprintOff();
        }
        // (테스트)위치 초기화
        if(Input.GetKeyDown(KeyCode.Alpha0))
        {
            transform.position = Vector3.up;
        }
        // (테스트)사다리 앞으로 이동
        if(Input.GetKeyDown(KeyCode.Alpha1))
        {
            transform.position = new Vector3(9, 2.8f, 11.5f);
        }

        // 마우스 회전 적용
        MouseRotation();
    }

    /// <summary>
    /// 마우스 회전으로 플레이어 회전 적용
    /// </summary>
    private void MouseRotation()
    {
        mouseX += Input.GetAxis("Mouse X") * rotateSpeed;
        mouseY = Mathf.Clamp(mouseY + Input.GetAxis("Mouse Y") * rotateSpeed, -limitAngle, limitAngle);

        movement3D.Rotation(0, mouseX);
        playerCamera.transform.rotation = Quaternion.Euler(playerCamera.transform.rotation.x - mouseY, transform.rotation.eulerAngles.y, 0);
    }

    /// <summary>
    /// 플레이어 사망 처리
    /// </summary>
    public void Death()
    {
        // 연속 사망 처리 방지
        if(!deathCoolTime)
        {
            // 사망 UI 업데이트 및 추가 이동하지 않도록 처리
            playManager.DeathUIEnable();
            rigidbody.useGravity = false;
            rigidbody.velocity = Vector3.zero;
            Debug.Log("Death!");
        }
    }

    /// <summary>
    /// 플레이어 리스폰 처리
    /// </summary>
    public void Respawn()
    {
        // 생존이 아닐 때에만 적용
        if(!alive)
        {
            // 생존으로 전환 및 관련 값 원상 복구
            alive = true;
            rigidbody.useGravity = true;
            // 시스템에 리스폰 알림
            playManager.OnRespawn();
            Debug.Log("Respawn!");
            // 연속 사망 방지 처리 쿨타임 적용
            StartCoroutine(DeathCoolTime());
        }
    }

    /// <summary>
    /// 연속 사망 처리 방지 코루틴
    /// </summary>
    IEnumerator DeathCoolTime()
    {
        deathCoolTime = true;
        yield return new WaitForSecondsRealtime(0.5f);

        deathCoolTime = false;
    }

    /// <summary>
    /// 스테이지 클리어 처리
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
    /// 생존 여부 설정
    /// </summary>
    /// <param name="alive">생존 여부</param>
    public void SetAlive(bool alive)
    {
        this.alive = alive;
    }
}
