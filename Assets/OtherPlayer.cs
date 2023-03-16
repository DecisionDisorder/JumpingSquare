using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class OtherPlayer : MonoBehaviour
{
    public string playerName { private set; get; }
    private Vector3 serverPosition = Vector3.zero;
    private Vector3 serverVelocity = Vector3.zero;
    private Vector3 serverRotation = Vector3.zero;

    private Rigidbody rigidbody;

    private Movement3D movement3D;
    public OtherPlayerName otherPlayerName;

    private void OnEnable()
    {
        if (rigidbody == null) { rigidbody = GetComponent<Rigidbody>(); }
        if (movement3D == null) { movement3D = GetComponent<Movement3D>(); }
    }

    private void Update()
    {
        if(serverPosition != Vector3.zero)
        {
            transform.rotation = Quaternion.Euler(serverRotation.x, serverRotation.y, serverRotation.z);
            // 프레임마다 움직인 결과를 serverposition에 반영하거나 따로 캐싱해서 움직인 결과 반영하기
            //movement3D.Move(serverMovement, serverPosition);
            //serverPosition = Vector3.zero;
            transform.position = GetNextPosition();
        }
        //else
            //movement3D.Move(serverMovement);
    }

    private Vector3 GetNextPosition()
    {
        return Vector3.Lerp(transform.position, serverPosition, 0.1f);
    }

    public void SetInfo(Vector3 pos, Vector3 vel, Vector3 rot)
    {
        serverPosition = pos;
        rigidbody.velocity = vel;
        serverRotation = rot;
    }

    public void SetName(string name)
    {
        playerName = name;
        otherPlayerName.SetName(name);
    }
}
