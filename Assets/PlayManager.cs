using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// ���� �÷��� ���� ���� Ŭ����
/// </summary>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// �ʱ� ���� UI �׷�
    /// </summary>
    public GameObject initialSettingGroup;
    /// <summary>
    /// ��� UI �׷�
    /// </summary>
    public GameObject deathUIGroup;
    /// <summary>
    /// Ŭ���� UI �׷�
    /// </summary>
    public GameObject clearUIGroup;
    /// <summary>
    /// �̸� �Է� �ʵ�
    /// </summary>
    public InputField nameInputField;
    /// <summary>
    /// ��Ʈ��ȣ �Է� �ʵ�
    /// </summary>
    public InputField portInputField;

    /// <summary>
    /// �¶���/�������� ���� ��ư
    /// </summary>
    public Button onlineButton;
    /// <summary>
    /// �¶���/�������� ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text onlineButtonText;

    /// <summary>
    /// ������ ����� ������Ʈ
    /// </summary>
    public CommunicateManager communicateManager;

    /// <summary>
    /// �������� �׽�Ʈ ����
    /// </summary>
    internal static bool OfflineTest { get; private set; }

    /// <summary>
    /// �� ���� ��ư
    /// </summary>
    public Button[] mapButtons;
    /// <summary>
    /// ������ �ʿ� ������ ����
    /// </summary>
    public Color selectedColor;

    /// <summary>
    /// ���õ� ��
    /// </summary>
    internal static int SelectedMap { get; private set; }
    /// <summary>
    /// ���� �Ϸ� ����
    /// </summary>
    private static bool settingCompleted = false;

    private void Awake()
    {
        // ���� ���� ���� �ƴ� ��쿡, ������ �Ǿ��ִ� ���°� �ƴϸ� �������� �׽�Ʈ�� ����
        if (!initialSettingGroup.activeInHierarchy)
        {
            if (!settingCompleted)
                OfflineTest = true;
        }
    }

    private void Start()
    {
        // �ִ� ������ 144�� ����
        Application.targetFrameRate = 144;
        // ���� ������ �ƴ� ���, ���콺 ���
        if(!initialSettingGroup.activeInHierarchy)
        {
            if (!settingCompleted)
                OfflineTest = true;
            MouseLock();
        }
        else
        {
            // �� ��ư ���� �� �¶��� ���� ��ư �ʱ�ȭ
            SelectMap(SelectedMap);
            SetOnlineButtonText();
        }
            
    }

    /// <summary>
    /// ���콺 ��� ó��
    /// </summary>
    private void MouseLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// ���콺 ��� ����
    /// </summary>
    private void MouseUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// �¶���/�������� UI ����
    /// </summary>
    private void SetOnlineButtonText()
    {
        if (!OfflineTest) { 
            onlineButtonText.text = "�¶���"; 
            onlineButtonText.color = Color.white;
            onlineButton.image.color = Color.blue;
        }
        else { 
            onlineButtonText.text = "��������";
            onlineButtonText.color = Color.black;
            onlineButton.image.color = Color.red;
        }
    }
    
    /// <summary>
    /// �� ����
    /// </summary>
    /// <param name="index">�� index</param>
    public void SelectMap(int index)
    {
        SelectedMap = index;
        // �� ��ư ���� ����
        for(int i = 0; i < mapButtons.Length; i++)
        {
            if(i == index)
                mapButtons[i].image.color = selectedColor;
            else
                mapButtons[i].image.color = Color.white;
        }
    }

    /// <summary>
    /// ���� �÷��� ����
    /// </summary>
    public void PlayGame()
    {
        settingCompleted = true;
        ApplyNameAndPort();
        SceneManager.LoadScene(SelectedMap + 1);
    }

    /// <summary>
    /// �¶��� ��� ����
    /// </summary>
    public void SetOnline()
    {
        OfflineTest = !OfflineTest;
        SetOnlineButtonText();
    }

    /// <summary>
    /// �÷��̾� �̸� ����
    /// </summary>
    public void ApplyNameAndPort()
    {
        string name = nameInputField.text;
        // �̸��� ����ִ� ��� ������ ��ȣ �ο�
        if (name == "" || name == null)
            name = "Player" + UnityEngine.Random.Range(0, 100);

        // ��Ʈ ��ȣ �о����
        int port = int.Parse(portInputField.text);
        
        // �̸� �� ��Ʈ��ȣ ����
        if(!OfflineTest)
        {
            communicateManager.SetPlayerName(name);
            communicateManager.SetUdpPort(port);
        }
    }

    /// <summary>
    /// ��� UI Ȱ��ȭ
    /// </summary>
    public void DeathUIEnable()
    {
        MouseUnlock();
        deathUIGroup.SetActive(true);
    }

    /// <summary>
    /// ������ ó��
    /// </summary>
    public void OnRespawn()
    {
        MouseLock();
        deathUIGroup.SetActive(false);
    }

    /// <summary>
    /// ������ ��û
    /// </summary>
    public void RequestRespawn()
    {
        deathUIGroup.SetActive(false);
        communicateManager.SendTCP(MessageData.instance.Respawn.Request);
    }

    /// <summary>
    /// �������� Ŭ���� ó��
    /// </summary>
    public void OnStageClear()
    {
        MouseUnlock();
        clearUIGroup.SetActive(true);
    }

    /// <summary>
    /// Ŭ���� UI ����
    /// </summary>
    public void CloseClearUI()
    {
        MouseLock();
        clearUIGroup.SetActive(false);
    }

    /// <summary>
    /// ���� ȭ������ ���ư���
    /// </summary>
    public void ReturnToEntrance()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// ���� �ð��� unix timestamp�� ms ������ �ҷ�����
    /// </summary>
    /// <returns>���� �ð� (ms)</returns>
    public static long GetCurrentTimeInMilliseconds()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}
