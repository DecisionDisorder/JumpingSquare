using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OtherPlayerName : MonoBehaviour
{
    private Camera mainCamera;
    private TextMesh playerNameTextMesh;

    private void OnEnable()
    {
        if(mainCamera == null)
            mainCamera = Camera.main;
        if(playerNameTextMesh == null)
            playerNameTextMesh = GetComponent<TextMesh>();
    }

    private void Update()
    {
        transform.rotation = mainCamera.gameObject.transform.rotation;
    }

    public void SetName(string name)
    {
        playerNameTextMesh.text = name;
    }
}
