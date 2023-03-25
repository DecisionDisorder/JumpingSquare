using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TCP �޽��� ������ �ҷ����� Ŭ����
/// </summary>
[System.Serializable]
public class MessageData
{
    /// <summary>
    /// ���� ó�� ���� �޽���
    /// </summary>
    [SerializeField]
    private ResponsiveMessage access;
    /// <summary>
    /// ������ ó�� ���� �޽���
    /// </summary>
    [SerializeField]
    private ResponsiveMessage respawn;
    /// <summary>
    /// ���� ���� �޽���
    /// </summary>
    [SerializeField]
    private string close;
    /// <summary>
    /// ��� ó�� �޽���
    /// </summary>
    [SerializeField]
    private string death;
    /// <summary>
    /// Ŭ���� ó�� �޽���
    /// </summary>
    [SerializeField]
    private string clear;
    /// <summary>
    /// ��ġ ������Ʈ �޽���
    /// </summary>
    [SerializeField]
    private string update;

    /// <summary>
    /// ���� ó�� ���� �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public ResponsiveMessage Access { get { return access; } }
    /// <summary>
    /// ������ ���� �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public ResponsiveMessage Respawn { get { return respawn; } }

    /// <summary>
    /// ���� ���� �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public string Close { get { return close; } }
    /// <summary>
    /// ��� ó�� �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public string Death { get { return death; } }
    /// <summary>
    /// Ŭ���� ó�� �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public string Clear { get { return clear; } }
    /// <summary>
    /// ��ġ ������Ʈ �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public string UpdatePosition { get { return update; } }

    /// <summary>
    /// �޽��� �����Ϳ� ������ �̱��� �ν��Ͻ�
    /// </summary>
    public static MessageData instance;

    /// <summary>
    /// ���Ϸκ��� �޽��� �����͸� �о�´�.
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
/// ��/������ �̷������ �޽��� Ŭ����
/// </summary>
[System.Serializable]
public class ResponsiveMessage
{
    /// <summary>
    /// ��û �޽���
    /// </summary>
    [SerializeField]
    private string request;
    /// <summary>
    /// ���� �޽���
    /// </summary>
    [SerializeField]
    private string accept;

    /// <summary>
    /// ��û �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public string Request { get { return request; } }
    /// <summary>
    /// ���� �޽��� (���ٿ� ������Ƽ)
    /// </summary>
    public string Accept { get { return accept; } }
}

