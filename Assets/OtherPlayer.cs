using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/// <summary>
/// 타 플레이어 이동 관리 클래스
/// </summary>
public class OtherPlayer : MonoBehaviour
{
    /// <summary>
    /// 타 플레이어 이름
    /// </summary>
    public string playerName { private set; get; }
    /// <summary>
    /// 타 플레이어가 서버에 데이터를 보낸 시간
    /// </summary>
    private long serverTimestamp;
    /// <summary>
    /// 서버에서 수신 받은 좌표
    /// </summary>
    private Vector3 serverPosition = Vector3.zero;
    /// <summary>
    /// 서버에서 수신 받은 회전 값
    /// </summary>
    private Vector3 serverRotation = Vector3.zero;

    /// <summary>
    /// 타 플레이어 오브젝트의 물리 처리 오브젝트
    /// </summary>
    private Rigidbody rigidbody;

    /// <summary>
    /// 타 플레이어 이동 관리 오브젝트
    /// </summary>
    private Movement3D movement3D;
    /// <summary>
    /// 타 플레이어 이름 표기 UI 관리 오브젝트
    /// </summary>
    public OtherPlayerName otherPlayerName;

    /// <summary>
    /// 활성화 될 때 컴포넌트 불러오기
    /// </summary>
    private void OnEnable()
    {
        if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
        if (movement3D == null) { movement3D = GetComponent<Movement3D>(); }
    }

    /// <summary>
    /// 매 프레임 마다 타 플레이어 위치 및 회전 값 적용
    /// </summary>
    private void Update()
    {
        // 플레이어 데이터를 수신 받은 것이 있을 때
        if(serverPosition != Vector3.zero)
        {
            // 회전 적용
            transform.rotation = Quaternion.Euler(serverRotation.x, serverRotation.y, serverRotation.z);
            // 위치 적용
            transform.position = GetNextPosition();
        }
    }

    /// <summary>
    /// 이번 프레임에서의 위치를 계산한다
    /// </summary>
    /// <returns>플레이어 위치</returns>
    private Vector3 GetNextPosition()
    {
        // 흐른 시간 계산
        long timePassed = PlayManager.GetCurrentTimeInMilliseconds() - serverTimestamp;
        // 초 단위로 변환
        float timePassedInSeconds = timePassed * 0.001f;
        Debug.Log("Delay: " + timePassedInSeconds + "s");
        // 선형 보간법에 따라 플레이어 위치 계산
        return Vector3.Lerp(transform.position, serverPosition, timePassedInSeconds);
    }

    /// <summary>
    /// 타 플레이어 정보 갱신
    /// </summary>
    /// <param name="time">데이터의 시간</param>
    /// <param name="pos">위치</param>
    /// <param name="rot">회전 값</param>
    public void SetInfo(long time, Vector3 pos,  Vector3 rot)
    {
        serverTimestamp = time;
        serverPosition = pos;
        serverRotation = rot;
    }

    /// <summary>
    /// 타 플레이어 이름 설정
    /// </summary>
    /// <param name="name">플레이어 이름</param>
    public void SetName(string name)
    {
        playerName = name;
        otherPlayerName.SetName(name);
    }
}
