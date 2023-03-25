using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UDP�� ����� �÷��̾� ������
/// </summary>
[System.Serializable]
public class PlayerState
{
    /// <summary>
    /// ������ ���� �ð�
    /// </summary>
    public long timestamp;
    /// <summary>
    /// �÷��̾� �̸�
    /// </summary>
    public string playerName;

    /// <summary>
    /// ���� ����
    /// </summary>
    public bool alive;
    /// <summary>
    /// �� ����
    /// </summary>
    public int map;
    /// <summary>
    /// ���� �̵� ����
    /// </summary>
    public bool forceTransform;

    /// <summary>
    /// �÷��̾� ��ġ X
    /// </summary>
    public float positionX;
    /// <summary>
    /// �÷��̾� ��ġ Y
    /// </summary>
    public float positionY;
    /// <summary>
    /// �÷��̾� ��ġ Z
    /// </summary>
    public float positionZ;

    /// <summary>
    /// �÷��̾� ȸ�� �� X
    /// </summary>
    public float rotationX;
    /// <summary>
    /// �÷��̾� ȸ�� �� Y
    /// </summary>
    public float rotationY;
    /// <summary>
    /// �÷��̾� ȸ�� �� Z
    /// </summary>
    public float rotationZ;
}

/// <summary>
/// TCP�� ����� �޽���
/// </summary>
[System.Serializable]
public class PlayerMessage
{
    /// <summary>
    /// �÷��̾� �̸�
    /// </summary>
    public string playerName;
    /// <summary>
    /// �޽���
    /// </summary>
    public string message;
}
