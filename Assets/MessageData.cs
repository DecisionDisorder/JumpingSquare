using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TCP 메시지 내용을 불러오는 클래스
/// </summary>
[System.Serializable]
public class MessageData
{
    /// <summary>
    /// 접속 처리 관련 메시지
    /// </summary>
    [SerializeField]
    private ResponsiveMessage access;
    /// <summary>
    /// 리스폰 처리 관련 메시지
    /// </summary>
    [SerializeField]
    private ResponsiveMessage respawn;
    /// <summary>
    /// 접속 종료 메시지
    /// </summary>
    [SerializeField]
    private string close;
    /// <summary>
    /// 사망 처리 메시지
    /// </summary>
    [SerializeField]
    private string death;
    /// <summary>
    /// 클리어 처리 메시지
    /// </summary>
    [SerializeField]
    private string clear;
    /// <summary>
    /// 위치 업데이트 메시지
    /// </summary>
    [SerializeField]
    private string update;

    /// <summary>
    /// 접속 처리 관련 메시지 (접근용 프로퍼티)
    /// </summary>
    public ResponsiveMessage Access { get { return access; } }
    /// <summary>
    /// 리스폰 관련 메시지 (접근용 프로퍼티)
    /// </summary>
    public ResponsiveMessage Respawn { get { return respawn; } }

    /// <summary>
    /// 접속 종료 메시지 (접근용 프로퍼티)
    /// </summary>
    public string Close { get { return close; } }
    /// <summary>
    /// 사망 처리 메시지 (접근용 프로퍼티)
    /// </summary>
    public string Death { get { return death; } }
    /// <summary>
    /// 클리어 처리 메시지 (접근용 프로퍼티)
    /// </summary>
    public string Clear { get { return clear; } }
    /// <summary>
    /// 위치 업데이트 메시지 (접근용 프로퍼티)
    /// </summary>
    public string UpdatePosition { get { return update; } }

    /// <summary>
    /// 메시지 데이터에 접근할 싱글톤 인스턴스
    /// </summary>
    public static MessageData instance;

    /// <summary>
    /// 파일로부터 메시지 데이터를 읽어온다.
    /// </summary>
    /// <returns></returns>
    public static MessageData ReadMessageDataFromFile()
    {
        var loadedJson = Resources.Load<TextAsset>("Json/message");
        MessageData data = JsonUtility.FromJson<MessageData>(loadedJson.ToString());

        return data;
    }

}

/// <summary>
/// 송/수신이 이루어지는 메시지 클래스
/// </summary>
[System.Serializable]
public class ResponsiveMessage
{
    /// <summary>
    /// 요청 메시지
    /// </summary>
    [SerializeField]
    private string request;
    /// <summary>
    /// 승인 메시지
    /// </summary>
    [SerializeField]
    private string accept;

    /// <summary>
    /// 요청 메시지 (접근용 프로퍼티)
    /// </summary>
    public string Request { get { return request; } }
    /// <summary>
    /// 승인 메시지 (접근용 프로퍼티)
    /// </summary>
    public string Accept { get { return accept; } }
}

