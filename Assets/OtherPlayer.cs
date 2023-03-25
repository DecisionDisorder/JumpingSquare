using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

/// <summary>
/// Ÿ �÷��̾� �̵� ���� Ŭ����
/// </summary>
public class OtherPlayer : MonoBehaviour
{
    /// <summary>
    /// Ÿ �÷��̾� �̸�
    /// </summary>
    public string playerName { private set; get; }
    /// <summary>
    /// Ÿ �÷��̾ ������ �����͸� ���� �ð�
    /// </summary>
    private long serverTimestamp;
    /// <summary>
    /// �������� ���� ���� ��ǥ
    /// </summary>
    private Vector3 serverPosition = Vector3.zero;
    /// <summary>
    /// �������� ���� ���� ȸ�� ��
    /// </summary>
    private Vector3 serverRotation = Vector3.zero;

    /// <summary>
    /// Ÿ �÷��̾� ������Ʈ�� ���� ó�� ������Ʈ
    /// </summary>
    private Rigidbody rigidbody;

    /// <summary>
    /// Ÿ �÷��̾� �̵� ���� ������Ʈ
    /// </summary>
    private Movement3D movement3D;
    /// <summary>
    /// Ÿ �÷��̾� �̸� ǥ�� UI ���� ������Ʈ
    /// </summary>
    public OtherPlayerName otherPlayerName;

    /// <summary>
    /// Ȱ��ȭ �� �� ������Ʈ �ҷ�����
    /// </summary>
    private void OnEnable()
    {
        if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
        if (movement3D == null) { movement3D = GetComponent<Movement3D>(); }
    }

    /// <summary>
    /// �� ������ ���� Ÿ �÷��̾� ��ġ �� ȸ�� �� ����
    /// </summary>
    private void Update()
    {
        // �÷��̾� �����͸� ���� ���� ���� ���� ��
        if(serverPosition != Vector3.zero)
        {
            // ȸ�� ����
            transform.rotation = Quaternion.Euler(serverRotation.x, serverRotation.y, serverRotation.z);
            // ��ġ ����
            transform.position = GetNextPosition();
        }
    }

    /// <summary>
    /// �̹� �����ӿ����� ��ġ�� ����Ѵ�
    /// </summary>
    /// <returns>�÷��̾� ��ġ</returns>
    private Vector3 GetNextPosition()
    {
        // �帥 �ð� ���
        long timePassed = PlayManager.GetCurrentTimeInMilliseconds() - serverTimestamp;
        // �� ������ ��ȯ
        float timePassedInSeconds = timePassed * 0.001f;
        Debug.Log("Delay: " + timePassedInSeconds + "s");
        // ���� �������� ���� �÷��̾� ��ġ ���
        return Vector3.Lerp(transform.position, serverPosition, timePassedInSeconds);
    }

    /// <summary>
    /// Ÿ �÷��̾� ���� ����
    /// </summary>
    /// <param name="time">�������� �ð�</param>
    /// <param name="pos">��ġ</param>
    /// <param name="rot">ȸ�� ��</param>
    public void SetInfo(long time, Vector3 pos,  Vector3 rot)
    {
        serverTimestamp = time;
        serverPosition = pos;
        serverRotation = rot;
    }

    /// <summary>
    /// Ÿ �÷��̾� �̸� ����
    /// </summary>
    /// <param name="name">�÷��̾� �̸�</param>
    public void SetName(string name)
    {
        playerName = name;
        otherPlayerName.SetName(name);
    }
}
