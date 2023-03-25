using UnityEngine;

/// <summary>
/// Ÿ �÷��̾� �̸� ǥ�� Ŭ����
/// </summary>
public class OtherPlayerName : MonoBehaviour
{
    /// <summary>
    /// ���� ī�޶�
    /// </summary>
    private Camera mainCamera;
    /// <summary>
    /// �÷��̾� �̸��� ǥ���� �ؽ�Ʈ
    /// </summary>
    private TextMesh playerNameTextMesh;

    /// <summary>
    /// Ȱ��ȭ �� ������Ʈ �ҷ�����
    /// </summary>
    private void OnEnable()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;
        if(playerNameTextMesh == null)
            playerNameTextMesh = GetComponent<TextMesh>();
    }

    /// <summary>
    /// �ؽ�Ʈ�� �׻� ī�޶� ������ ������Ʈ
    /// </summary>
    private void Update()
    {
        transform.rotation = mainCamera.gameObject.transform.rotation;
    }

    /// <summary>
    /// �ش� �÷��̾� �̸� ����
    /// </summary>
    /// <param name="name">�÷��̾� �̸�</param>
    public void SetName(string name)
    {
        playerNameTextMesh.text = name;
    }
}
