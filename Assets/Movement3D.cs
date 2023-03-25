using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

/// <summary>
/// 플레이어 이동 시스템 관리 클래스
/// </summary>
public class Movement3D : MonoBehaviour
{
    /// <summary>
    /// 물리 처리 Rigidbody
    /// </summary>
    public Rigidbody rigidbody;

    /// <summary>
    /// 이동 방향
    /// </summary>
    [SerializeField]
    private Vector3 moveDirection;
    /// <summary>
    /// 달리기 속도
    /// </summary>
    [SerializeField]
    private float sprintSpeed = 15.0f;
    /// <summary>
    /// 걷기 속도
    /// </summary>
    [SerializeField]
    private float walkSpeed = 10.0f;

    /// <summary>
    /// 이동 속도 (달리기/걷기 중에 상태에 맞추어 반환)
    /// </summary>
    private float moveSpeed { get { return isSprint ? sprintSpeed : walkSpeed; } }
    /// <summary>
    /// 점프시 이동속도
    /// </summary>
    [SerializeField]
    private float jumpMoveSpeed = 10.0f;
    /// <summary>
    /// 점프 Force
    /// </summary>
    [SerializeField]
    private float jumpForce = 3.0f;
    /// <summary>
    /// 사다리 오르기 속도
    /// </summary>
    [SerializeField]
    private float climbingSpeed = 1.0f;

    /// <summary>
    /// 사용자의 입력 방향
    /// </summary>
    [SerializeField]
    private Vector3 inputDirection;

    /// <summary>
    /// 달리는 중인지 여부
    /// </summary>
    [SerializeField]
    private bool isSprint = false;
    /// <summary>
    /// 점프하는 중인지 여부
    /// </summary>
    [SerializeField]
    private bool isJumping = false;
    /// <summary>
    /// 사다리를 타는 중인지 여부
    /// </summary>
    [SerializeField]
    private bool isClimbing = false;

    /// <summary>
    /// 사다리 타기 코루틴 캐시 오브젝트
    /// </summary>
    IEnumerator climbingCoroutine = null;

    void Awake()
    {
        if (rigidbody == null)
            rigidbody = GetComponent<Rigidbody>();
    }

    /// <summary>
    /// 이동 방향에 따라서 이동 처리
    /// </summary>
    /// <param name="direction">이동 방향</param>
    public void Move(Vector3 direction)
    {
        Move(direction, transform.position);
    }
    /// <summary>
    /// 기준 점에서 이동 처리
    /// </summary>
    /// <param name="direction">이동 방향</param>
    /// <param name="position">기준점</param>
    public void Move(Vector3 direction, Vector3 position)
    {
        // 사다리 등반 중이 아니면
        if(!isClimbing)
        {
            // 수직/수평 방향 입력값에 현재 회전중인 방향 값 적용
            Vector3 moveHorizontal = transform.right * direction.x;
            Vector3 moveVertical = transform.forward * direction.z;

            // 입력값을 하나의 벡터로 합치고 이동 속도까지 합산하여 이동 방향 계산
            moveDirection = (moveHorizontal + moveVertical).normalized * moveSpeed;

            // 이동 적용
            rigidbody.MovePosition(position + moveDirection * Time.deltaTime);
        }
        else
        {
            if (climbingCoroutine == null)
                StartCoroutine(climbingCoroutine = Climbing());
        }
    }

    /// <summary>
    /// 입력 방향 설정
    /// </summary>
    /// <param name="direction">입력 방향</param>
    public void SetInputDirection(Vector3 direction)
    {
        inputDirection = direction;
    }

    /// <summary>
    /// 플레이어 점프
    /// </summary>
    public void Jump()
    {
        // 점프 중이 아닐 때에만 허용
        if(!isJumping)
        {
            // 점프 상태로 변경 후 수직방향으로 순간적인 힘을 가한다
            isJumping = true;
            rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    /// <summary>
    /// 달리기 On
    /// </summary>
    public void SprintOn()
    {
        isSprint = true;

    }

    /// <summary>
    /// 달리기 Off
    /// </summary>
    public void SprintOff()
    {
        isSprint = false;
    }

    /// <summary>
    /// 사다리 등반 처리 코루틴
    /// </summary>
    IEnumerator Climbing()
    {
        yield return new WaitForEndOfFrame();

        // 앞으로 가는 버튼을 누르고 있을 때 상승
        if(inputDirection.z == 1)
            rigidbody.MovePosition(transform.position + Vector3.up * climbingSpeed * Time.deltaTime);
        // 앞으로 안갈때는 하강
        else
            rigidbody.MovePosition(transform.position - Vector3.up * climbingSpeed * Time.deltaTime);

        if (isClimbing) { StartCoroutine(Climbing()); }
        else { climbingCoroutine = null; }
    }

    /// <summary>
    /// 플레이어 충돌 처리
    /// </summary>
    /// <param name="collision">충돌체</param>
    private void OnCollisionEnter(Collision collision)
    {
        // 땅과 충돌하면 점프 종료 처리
        if(collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
        // 사다리와 충돌하면 등반 처리 및 그에 필요한 값 설정
        else if(collision.gameObject.CompareTag("Ladder"))
        {
            rigidbody.velocity = Vector3.zero;
            rigidbody.useGravity = false;
            isClimbing = true;
        }
    }

    /// <summary>
    /// 플레이어 충돌이 끝날 때의 처리
    /// </summary>
    /// <param name="collision">충돌체</param>
    private void OnCollisionExit(Collision collision)
    {
        // 사다리에서 떨어지면, 등반 모드 취소 및 원 상태 복구
        if (collision.gameObject.CompareTag("Ladder"))
        {
            rigidbody.useGravity = true;
            isClimbing = false;
        }
    }

    /// <summary>
    /// 플레이어 회전 적용
    /// </summary>
    /// <param name="rotateX">x축 회전 값</param>
    /// <param name="rotateY">y축 회전 값</param>
    public void Rotation(float rotateX, float rotateY)
    {
        rigidbody.rotation = Quaternion.Euler(transform.rotation.x - rotateX, transform.rotation.y + rotateY, 0.0f);
    }
}
