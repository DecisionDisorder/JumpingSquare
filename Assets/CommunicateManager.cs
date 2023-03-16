using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;



public class CommunicateManager : MonoBehaviour
{
    private Socket client;
    private static string host = "211.221.158.44";
    private static int port = 48080;

    //public ToServerPacket sendPacket = new ToServerPacket();
    public PlayerState sendState = new PlayerState();
    //public FromServerPacket recvPacket = new FromServerPacket();
    public PlayerState[] receiveStates;
    public PlayerState receivePlayerState;
    private IPEndPoint serverIpEndPoint;
    private EndPoint remoteEndPoint;

    public Player player;
    public OtherPlayer otherPlayerSample;
    [SerializeField]
    private List<OtherPlayer> otherPlayers;
    public Transform otherPlayerGroup;

    private static string playerName;
    private string otherMessage = "";
    private bool persistent = false;

    private void Start()
    {
        if(!PlayManager.OfflineTest)
            InitClient();
    }

    private IEnumerator Communicate()
    {
        if (client != null)
        {
            Send();
            Receive();
        }

        yield return new WaitForSeconds(0.1f);

        StartCoroutine(Communicate());
    }


    private void OnApplicationQuit()
    {
        CloseClient();
    }

    public void InitClient()
    {
        serverIpEndPoint = new IPEndPoint(IPAddress.Parse(host), port);
        client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        client.Connect(serverIpEndPoint);

        StartCoroutine(Communicate());
    }

    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    private void SetSendPacket()
    {
        sendState.playerName = playerName;

        sendState.alive = player.alive;
        sendState.map = PlayManager.SelectedMap;

        sendState.positionX = player.transform.position.x;
        sendState.positionY = player.transform.position.y;
        sendState.positionZ = player.transform.position.z;

        sendState.rotationX = player.transform.rotation.eulerAngles.x;
        sendState.rotationY = player.transform.rotation.eulerAngles.y;
        sendState.rotationZ = player.transform.rotation.eulerAngles.z;

        sendState.velocityX = player.rigidbody.velocity.x;
        sendState.velocityY = player.rigidbody.velocity.y;
        sendState.velocityZ = player.rigidbody.velocity.z;

        // TODO : 메시지 내용 따로 관리
        if (otherMessage.Equals(""))
            sendState.message = "UpdatePosition";
        else
        {
            sendState.message = otherMessage;
            if(!persistent)
            {
                //Debug.Log("Message Reset");
                otherMessage = "";
            }
        }
        //Debug.Log("[Sending Message] " + sendState.message);
    }

    private void Send()
    {
        try
        {
            SetSendPacket();

            //byte[] sendPacketBytes = StructToByteArray(sendPacket);
            var sendStateJson = JsonUtility.ToJson(sendState);
            var sendPacketBytes = Encoding.UTF8.GetBytes(sendStateJson + "|");
            //Debug.Log(sendStateJson);
            client.Send(sendPacketBytes, 0, sendPacketBytes.Length, SocketFlags.None);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }
    }

    public void SetOtherMessage(string message, bool persistent = false)
    {
        Debug.Log("Message Updated : " + message + " / persistent: " + persistent);
        this.persistent = persistent;
        otherMessage = message;
    }

    public void ResetMessage()
    {
        otherMessage = "";
        Debug.Log("Message Reset (Func)");
    }

    private void Receive()
    {
        int receive = 0;
        if(client.Available != 0)
        {
            byte[] packet = new byte[1024];
            try
            {
                receive = client.Receive(packet);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            string recvJson = Encoding.UTF8.GetString(packet);
            string[] jsons = recvJson.Split('|');

            try
            {
                PlayerState[] receiveData = JsonHelper.FromJson<PlayerState>(jsons[jsons.Length - 1]);
                receiveStates = new PlayerState[receiveData.Length - 1];
                for(int i = 0, j = 0; i < receiveData.Length; i++)
                {
                    if (receiveData[i].playerName.Equals(playerName))
                        receivePlayerState = receiveData[i];
                    else
                        receiveStates[j++] = receiveData[i];
                }

            }
            catch(Exception e)
            {
                Debug.LogError(e.StackTrace);
                Debug.Log("Size:" + receive);
            }
            
            if (receive > 0)
            {
                ApplyToClient();
            }
        }
    }

    private OtherPlayer GetOtherPlayerByName(string name)
    {
        foreach(OtherPlayer fobj in otherPlayers)
        {
            if (fobj.playerName.Equals(name))
                return fobj;
        }

        return null;
    }

    private void AddOtherPlayer()
    {
        for (int i = otherPlayers.Count; i < receiveStates.Length; i++)
        {
            otherPlayers.Add(Instantiate(otherPlayerSample, otherPlayerGroup));
            otherPlayers[i].gameObject.SetActive(true);
            otherPlayers[i].SetName(receiveStates[i].playerName);
        }
    }

    private void SetPlayerData()
    {
        string message = receivePlayerState.message;
        // TODO : 메시지 내용을 다른 형식으로 관리하기 (json 등)
        if (message.Equals("Death"))
        {
            player.Death();
        }
        else if (message.Equals("RespawnAccepted"))
        {
            Vector3 pos = new Vector3(receivePlayerState.positionX, receivePlayerState.positionY, receivePlayerState.positionZ);
            player.Respawn(pos);
        }
        else if (message.Equals("StageClear"))
        {
            player.StageClear();
        }
    }

    private void ApplyToClient()
    {
        SetPlayerData();

        if (otherPlayers.Count < receiveStates.Length)
            AddOtherPlayer();

        for(int i = 0; i < receiveStates.Length; i++)
        {
            OtherPlayer otherPlayer = GetOtherPlayerByName(receiveStates[i].playerName);

            Vector3 pos = new Vector3(receiveStates[i].positionX, receiveStates[i].positionY, receiveStates[i].positionZ);
            Vector3 vel = new Vector3(receiveStates[i].velocityX, receiveStates[i].velocityY, receiveStates[i].velocityZ);
            Vector3 rot = new Vector3(receiveStates[i].rotationX, receiveStates[i].rotationY, receiveStates[i].rotationZ);
            otherPlayer.SetInfo(pos, vel, rot);
        }
    }

    private void CloseClient()
    {
        if(client != null )
        {
            client.Close();
            client = null;
        }
    }
}
