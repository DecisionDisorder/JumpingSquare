using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayManager : MonoBehaviour
{
    public GameObject initialSettingGroup;
    public GameObject deathUIGroup;
    public GameObject clearUIGroup;
    public InputField nameInputField;

    public Button onlineButton;
    public Text onlineButtonText;

    public CommunicateManager communicateManager;

    internal static bool OfflineTest { get; private set; }

    public Button[] mapButtons;
    public Color selectedColor;
    internal static int SelectedMap { get; private set; }
    private static bool settingCompleted = false;

    private void Awake()
    {
        if (!initialSettingGroup.activeInHierarchy)
        {
            if (!settingCompleted)
                OfflineTest = true;
        }
    }

    private void Start()
    {
        Application.targetFrameRate = 144;
        //initialSettingGroup.SetActive(true);
        if(!initialSettingGroup.activeInHierarchy)
        {
            if (!settingCompleted)
                OfflineTest = true;
            MouseLock();
        }
        else
        {
            SelectMap(SelectedMap);
            SetOnlineButtonText();
        }
            
    }

    private void MouseLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void MouseUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    private void SetOnlineButtonText()
    {
        if (!OfflineTest) { 
            onlineButtonText.text = "온라인"; 
            onlineButtonText.color = Color.white;
            onlineButton.image.color = Color.blue;
        }
        else { 
            onlineButtonText.text = "오프라인";
            onlineButtonText.color = Color.black;
            onlineButton.image.color = Color.red;
        }
    }

    public void SelectMap(int index)
    {
        SelectedMap = index;
        for(int i = 0; i < mapButtons.Length; i++)
        {
            if(i == index)
                mapButtons[i].image.color = selectedColor;
            else
                mapButtons[i].image.color = Color.white;
        }
    }

    public void PlayGame()
    {
        settingCompleted = true;
        ApplyName();
        SceneManager.LoadScene(SelectedMap + 1);
    }

    public void SetOnline()
    {
        OfflineTest = !OfflineTest;
        SetOnlineButtonText();
    }

    public void ApplyName()
    {
        string name = nameInputField.text;
        if (name == "" || name == null)
            name = "Player" + Random.Range(0, 100);

        if(!OfflineTest)
        {
            communicateManager.SetPlayerName(name);
        }
    }

    public void DeathUIEnable()
    {
        MouseUnlock();
        deathUIGroup.SetActive(true);
    }

    public void OnRespawn()
    {
        MouseLock();
        deathUIGroup.SetActive(false);
        communicateManager.SetOtherMessage("RespawnApplied");
    }

    public void RequestRespawn()
    {
        deathUIGroup.SetActive(false);
        // TODO : 메시지 모음 정리
        communicateManager.SetOtherMessage("RequestRespawn");
    }

    public void OnStageClear()
    {
        MouseUnlock();
        clearUIGroup.SetActive(true);
    }

    public void CloseClearUI()
    {
        MouseLock();
        clearUIGroup.SetActive(false);
    }

    public void ReturnToEntrance()
    {
        SceneManager.LoadScene(0);
    }
}
