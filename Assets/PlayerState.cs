using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UDP로 통신할 플레이어 데이터
/// </summary>
[System.Serializable]
public class PlayerState
{
    /// <summary>
    /// 서버에 보낸 시간
    /// </summary>
    public long timestamp;
    /// <summary>
    /// 플레이어 이름
    /// </summary>
    public string playerName;

    /// <summary>
    /// 생존 여부
    /// </summary>
    public bool alive;
    /// <summary>
    /// 맵 종류
    /// </summary>
    public int map;
    /// <summary>
    /// 강제 이동 여부
    /// </summary>
    public bool forceTransform;

    /// <summary>
    /// 플레이어 위치 X
    /// </summary>
    public float positionX;
    /// <summary>
    /// 플레이어 위치 Y
    /// </summary>
    public float positionY;
    /// <summary>
    /// 플레이어 위치 Z
    /// </summary>
    public float positionZ;

    /// <summary>
    /// 플레이어 회전 값 X
    /// </summary>
    public float rotationX;
    /// <summary>
    /// 플레이어 회전 값 Y
    /// </summary>
    public float rotationY;
    /// <summary>
    /// 플레이어 회전 값 Z
    /// </summary>
    public float rotationZ;
}

/// <summary>
/// TCP로 통신할 메시지
/// </summary>
[System.Serializable]
public class PlayerMessage
{
    /// <summary>
    /// 플레이어 이름
    /// </summary>
    public string playerName;
    /// <summary>
    /// 메시지
    /// </summary>
    public string message;
}
