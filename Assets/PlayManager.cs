using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

/// <summary>
/// 게임 플레이 관련 제어 클래스
/// </summary>
public class PlayManager : MonoBehaviour
{
    /// <summary>
    /// 초기 설정 UI 그룹
    /// </summary>
    public GameObject initialSettingGroup;
    /// <summary>
    /// 사망 UI 그룹
    /// </summary>
    public GameObject deathUIGroup;
    /// <summary>
    /// 클리어 UI 그룹
    /// </summary>
    public GameObject clearUIGroup;
    /// <summary>
    /// 이름 입력 필드
    /// </summary>
    public InputField nameInputField;
    /// <summary>
    /// 포트번호 입력 필드
    /// </summary>
    public InputField portInputField;

    /// <summary>
    /// 온라인/오프라인 설정 버튼
    /// </summary>
    public Button onlineButton;
    /// <summary>
    /// 온라인/오프라인 표기 텍스트
    /// </summary>
    public Text onlineButtonText;

    /// <summary>
    /// 서버와 통신할 오브젝트
    /// </summary>
    public CommunicateManager communicateManager;

    /// <summary>
    /// 오프라인 테스트 여부
    /// </summary>
    internal static bool OfflineTest { get; private set; }

    /// <summary>
    /// 맵 선택 버튼
    /// </summary>
    public Button[] mapButtons;
    /// <summary>
    /// 선택한 맵에 적용할 색상
    /// </summary>
    public Color selectedColor;

    /// <summary>
    /// 선택된 맵
    /// </summary>
    internal static int SelectedMap { get; private set; }
    /// <summary>
    /// 설정 완료 여부
    /// </summary>
    private static bool settingCompleted = false;

    private void Awake()
    {
        // 입장 설정 씬이 아닌 경우에, 설정이 되어있는 상태가 아니면 오프라인 테스트로 설정
        if (!initialSettingGroup.activeInHierarchy)
        {
            if (!settingCompleted)
                OfflineTest = true;
        }
    }

    private void Start()
    {
        // 최대 프레임 144로 제한
        Application.targetFrameRate = 144;
        // 입장 설정이 아닌 경우, 마우스 잠금
        if(!initialSettingGroup.activeInHierarchy)
        {
            if (!settingCompleted)
                OfflineTest = true;
            MouseLock();
        }
        else
        {
            // 맵 버튼 상태 및 온라인 설정 버튼 초기화
            SelectMap(SelectedMap);
            SetOnlineButtonText();
        }
            
    }

    /// <summary>
    /// 마우스 잠금 처리
    /// </summary>
    private void MouseLock()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    /// <summary>
    /// 마우스 잠금 해제
    /// </summary>
    private void MouseUnlock()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    /// <summary>
    /// 온라인/오프라인 UI 설정
    /// </summary>
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
    
    /// <summary>
    /// 맵 선택
    /// </summary>
    /// <param name="index">맵 index</param>
    public void SelectMap(int index)
    {
        SelectedMap = index;
        // 맵 버튼 색상 변경
        for(int i = 0; i < mapButtons.Length; i++)
        {
            if(i == index)
                mapButtons[i].image.color = selectedColor;
            else
                mapButtons[i].image.color = Color.white;
        }
    }

    /// <summary>
    /// 게임 플레이 시작
    /// </summary>
    public void PlayGame()
    {
        settingCompleted = true;
        ApplyNameAndPort();
        SceneManager.LoadScene(SelectedMap + 1);
    }

    /// <summary>
    /// 온라인 모드 설정
    /// </summary>
    public void SetOnline()
    {
        OfflineTest = !OfflineTest;
        SetOnlineButtonText();
    }

    /// <summary>
    /// 플레이어 이름 적용
    /// </summary>
    public void ApplyNameAndPort()
    {
        string name = nameInputField.text;
        // 이름이 비어있는 경우 랜덤한 번호 부여
        if (name == "" || name == null)
            name = "Player" + UnityEngine.Random.Range(0, 100);

        // 포트 번호 읽어오기
        int port = int.Parse(portInputField.text);
        
        // 이름 및 포트번호 설정
        if(!OfflineTest)
        {
            communicateManager.SetPlayerName(name);
            communicateManager.SetUdpPort(port);
        }
    }

    /// <summary>
    /// 사망 UI 활성화
    /// </summary>
    public void DeathUIEnable()
    {
        MouseUnlock();
        deathUIGroup.SetActive(true);
    }

    /// <summary>
    /// 리스폰 처리
    /// </summary>
    public void OnRespawn()
    {
        MouseLock();
        deathUIGroup.SetActive(false);
    }

    /// <summary>
    /// 리스폰 요청
    /// </summary>
    public void RequestRespawn()
    {
        deathUIGroup.SetActive(false);
        communicateManager.SendTCP(MessageData.instance.Respawn.Request);
    }

    /// <summary>
    /// 스테이지 클리어 처리
    /// </summary>
    public void OnStageClear()
    {
        MouseUnlock();
        clearUIGroup.SetActive(true);
    }

    /// <summary>
    /// 클리어 UI 종료
    /// </summary>
    public void CloseClearUI()
    {
        MouseLock();
        clearUIGroup.SetActive(false);
    }

    /// <summary>
    /// 입장 화면으로 돌아가기
    /// </summary>
    public void ReturnToEntrance()
    {
        SceneManager.LoadScene(0);
    }

    /// <summary>
    /// 현재 시간을 unix timestamp의 ms 단위로 불러오기
    /// </summary>
    /// <returns>현재 시간 (ms)</returns>
    public static long GetCurrentTimeInMilliseconds()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
    }
}
