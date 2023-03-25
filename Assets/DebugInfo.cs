using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 디버깅에 필요한 정보 관리 클래스
/// </summary>
public class DebugInfo : MonoBehaviour
{
    /// <summary>
    /// 폰트 크기 설정(10~150)
    /// </summary>
    [Range(10, 150)]
    public int fontSize = 30;
    /// <summary>
    /// 텍스트 색상
    /// </summary>
    public Color color = new Color();
    /// <summary>
    /// 텍스트 너비
    /// </summary>
    public float width;
    /// <summary>
    /// 텍스트 높이
    /// </summary>
    public float height;

    /// <summary>
    /// GUI 업데이트
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
