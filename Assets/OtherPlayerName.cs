using UnityEngine;

/// <summary>
/// 타 플레이어 이름 표기 클래스
/// </summary>
public class OtherPlayerName : MonoBehaviour
{
    /// <summary>
    /// 메인 카메라
    /// </summary>
    private Camera mainCamera;
    /// <summary>
    /// 플레이어 이름을 표시할 텍스트
    /// </summary>
    private TextMesh playerNameTextMesh;

    /// <summary>
    /// 활성화 때 컴포넌트 불러오기
    /// </summary>
    private void OnEnable()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;
        if(playerNameTextMesh == null)
            playerNameTextMesh = GetComponent<TextMesh>();
    }

    /// <summary>
    /// 텍스트가 항상 카메라를 보도록 업데이트
    /// </summary>
    private void Update()
    {
        transform.rotation = mainCamera.gameObject.transform.rotation;
    }

    /// <summary>
    /// 해당 플레이어 이름 설정
    /// </summary>
    /// <param name="name">플레이어 이름</param>
    public void SetName(string name)
    {
        playerNameTextMesh.text = name;
    }
}
