using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ����뿡 �ʿ��� ���� ���� Ŭ����
/// </summary>
public class DebugInfo : MonoBehaviour
{
    /// <summary>
    /// ��Ʈ ũ�� ����(10~150)
    /// </summary>
    [Range(10, 150)]
    public int fontSize = 30;
    /// <summary>
    /// �ؽ�Ʈ ����
    /// </summary>
    public Color color = new Color();
    /// <summary>
    /// �ؽ�Ʈ �ʺ�
    /// </summary>
    public float width;
    /// <summary>
    /// �ؽ�Ʈ ����
    /// </summary>
    public float height;

    /// <summary>
    /// GUI ������Ʈ
    /// </summary>
    private void OnGUI()
    {
        Rect position = new Rect(width, height, Screen.width, Screen.height);

        float fps = 1.0f / Time.deltaTime;
        float ms = Time.deltaTime * 1000f;
        string text = string.Format("{0:N1} FPS ({1:N1}ms)", fps, ms);

        GUIStyle style = new GUIStyle();

        style.fontSize = fontSize;
        style.normal.textColor = color;

        GUI.Label(position, text, style);

    }
}
