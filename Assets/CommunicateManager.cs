using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ���� ������ ���� ����ϴ� Ŭ����
/// </summary>
public class CommunicateManager : MonoBehaviour
{
    /// <summary>
    /// TCP ���� (�޽��� ��ſ�)
    /// </summary>
    private Socket tcpClient;
    /// <summary>
    /// UDP ���� (���� ������ ��ſ�)
    /// </summary>
    private UdpClient udpClient;
    
    /// <summary>
    /// ���� ���� IP
    /// </summary>
    private static string host = "172.30.1.49";
    /// <summary>
    /// TCP ��� ���� ��Ʈ ��ȣ
    /// </summary>
    private static int tcpPort = 49990;
    /// <summary>
    /// UDP ��� ���� ��Ʈ ��ȣ
    /// </summary>
    private static int udpPort = 49999;
    /// <summary>
    /// UDP ��� Ŭ���̾�Ʈ ��Ʈ ��ȣ
    /// </summary>
    private static int udpSendPort = 49995;
    /// <summary>
    /// �ִ� ���� ũ��
    /// </summary>
    private const int BUFFER_SIZE = 1024;

    /// <summary>
    /// ������ �÷��̾� ������
    /// </summary>
    public PlayerState sendState = new PlayerState();
    /// <summary>
    /// ���� ���� �ٸ� �÷��̾� ������
    /// </summary>
    public List<PlayerState> receiveStates;
    /// <summary>
    /// ���� ���� �ڽſ� ���� �÷��̾� ������
    /// </summary>
    public PlayerState receivePlayerState;
    /// <summary>
    /// ������ TCP �޽���
    /// </summary>
    public PlayerMessage sendMessage;
    /// <summary>
    /// ���� ���� TCP �޽���
    /// </summary>
    public PlayerMessage receiveMessage;

    /// <summary>
    /// TCP ���� End point
    /// </summary>
    private IPEndPoint tcpEndpoint;
    /// <summary>
    /// UDP ���� End point
    /// </summary>
    private IPEndPoint udpEndPoint;

    /// <summary>
    /// �÷��̾� ���� ������Ʈ
    /// </summary>
    public Player player;
    /// <summary>
    /// �ٸ� �÷��̾� ���� ������Ʈ
    /// </summary>
    public OtherPlayer otherPlayerSample;
    /// <summary>
    /// ��ȯ�� �ٸ� �÷��̾� ������Ʈ
    /// </summary>
    [SerializeField]
    private List<OtherPlayer> otherPlayers;
    /// <summary>
    /// �ٸ� �÷��̾��� ��ȯ �ʵ� Transform
    /// </summary>
    public Transform otherPlayerGroup;

    /// <summary>
    /// �÷��̾� �̸�
    /// </summary>
    private static string playerName;
    /// <summary>
    /// (�׽�Ʈ��)��Ʈ��ȣ ǥ�� �ؽ�Ʈ
    /// </summary>
    public Text portText;

    /// <summary>
    /// UDP ���� �α� ǥ�� ����
    /// </summary>
    public bool udpDebug;
    /// <summary>
    /// TCP ���� �α� ǥ�� ����
    /// </summary>
    public bool tcpDebug;

    private void Awake()
    {
        // �޽��� �����Ͱ� ��������� ���Ͽ��� �о�´�.
        if (MessageData.instance == null)
        {
            MessageData.instance = MessageData.ReadMessageDataFromFile();
        }
    }

    private void Start()
    {
        // �¶��� �׽�Ʈ�̸�, TCP ���ϰ� Ÿ �÷��̾� ����Ʈ �ʱ�ȭ�Ѵ�
        if(!PlayManager.OfflineTest)
        {
            InitTcpClient();
            otherPlayers = new List<OtherPlayer>();
        }       
    }

    /// <summary>
    /// �ֱ��� ��� �ڷ�ƾ
    /// </summary>
    private IEnumerator Communicate()
    {
        // UDP�� �÷��̾� �����͸� �ۼ���
        if (udpClient != null)
        {
            SendUDP();
            ReceiveUdp();
        }
        // TCP�� �޽��� ������ ����
        if(tcpClient != null)
            ReceiveTcp();

        // 0.1�� �������� �ݺ�
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(Communicate());
    }

    /// <summary>
    /// ���� ����� �� ������ �ݴ´�.
    /// </summary>
    private void OnApplicationQuit()
    {
        CloseClient();
    }

    /// <summary>
    /// TCP ���� �ʱ�ȭ �۾�
    /// </summary>
    private void InitTcpClient()
    {
        // TCP IP endpoint�� �ʱ�ȭ�ϰ�, TCP Ŭ���̾�Ʈ�� TCP �������� �ʱ�ȭ
        tcpEndpoint = new IPEndPoint(IPAddress.Parse(host), tcpPort);
        tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // ������ ���� ��û
        tcpClient.Connect(tcpEndpoint);

        // TCP ���� ���
        StartCoroutine(ReceiveTcpRetry());

        // �÷��̾� ���� ����
        SendTCP(playerName);
        // Non-blocking TCP socket���� ����
        tcpClient.Blocking = false;
    }

    /// <summary>
    /// UDP ���� �ʱ�ȭ �۾�
    /// </summary>
    private void InitUdpClient()
    {
        // UDP IP endpoint �� ���� �ʱ�ȭ
        udpEndPoint = new IPEndPoint(IPAddress.Parse(host), udpPort);
        udpClient = new UdpClient(udpSendPort);

        // �ֱ��� ��� ����
        StartCoroutine(Communicate());
    }

    /// <summary>
    /// ������ ������ �÷��̾� �̸� ����
    /// </summary>
    /// <param name="name">�÷��̾� �̸�</param>
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    /// <summary>
    /// UDP Ŭ���̾�Ʈ�� ��Ʈ ��ȣ ����
    /// </summary>
    /// <param name="port">Ŭ���̾�Ʈ UDP ��Ʈ ��ȣ</param>
    public void SetUdpPort(int port)
    {
        udpSendPort = port;
    }

    /// <summary>
    /// UDP�� �۽��� �÷��̾� ������ ����
    /// </summary>
    private void SetUdpSendPacket()
    {
        sendState.timestamp = PlayManager.GetCurrentTimeInMilliseconds();
        sendState.playerName = playerName;
        sendState.forceTransform = false;

        sendState.alive = player.alive;
        sendState.map = PlayManager.SelectedMap;

        sendState.positionX = player.transform.position.x;
        sendState.positionY = player.transform.position.y;
        sendState.positionZ = player.transform.position.z;

        sendState.rotationX = player.transform.rotation.eulerAngles.x;
        sendState.rotationY = player.transform.rotation.eulerAngles.y;
        sendState.rotationZ = player.transform.rotation.eulerAngles.z;
    }

    /// <summary>
    /// UDP �÷��̾� ������ ����
    /// </summary>
    private void SendUDP()
    {
        try
        {
            // ������ ����
            SetUdpSendPacket();

            // Json/byte array�� ��ȯ�Ͽ� UDP ������ ������ ����
            var sendStateJson = JsonUtility.ToJson(sendState);
            var sendPacketBytes = Encoding.UTF8.GetBytes(sendStateJson);
            udpClient.Send(sendPacketBytes, sendPacketBytes.Length, host, udpPort);
            if(udpDebug)
                Debug.Log("[UDP] Sended");
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }
    }

    /// <summary>
    /// UDP �÷��̾� ������ ����
    /// </summary>
    private void ReceiveUdp()
    {
        if(udpDebug)
            Debug.Log("UDP Client Available: " + udpClient.Available);

        // ���ۿ� ������ �����Ͱ� ���� ��
        if (udpClient.Available != 0)
        {
            // Ŭ���̾�Ʈ�� ��Ʈ ��ȣ ���� UI ������Ʈ
            string port = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port.ToString();
            portText.text = "Port: " + port;
            // ���Ź��� ������ �迭 �ʱ�ȭ
            byte[] packet = new byte[1024];
            try
            {
                // ������ ����
                packet = udpClient.Receive(ref udpEndPoint);
                if(udpDebug)
                    Debug.Log("[Receive] " + udpEndPoint.ToString() + "�κ��� " + packet.Length + "byte ����");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            // ���Ź��� byte array�� ���ڿ��� ��ȯ
            string recvJson = Encoding.UTF8.GetString(packet);

            try
            {
                // Json �����͸� PlayerState Ŭ������ ��ȯ (Deserialize)
                PlayerState[] receiveData = JsonHelper.FromJson<PlayerState>(recvJson);
                // ���Ź��� ����Ʈ�� �ʱ�ȭ���� �ʾ����� �ʱ�ȭ
                if (receiveStates == null)
                    receiveStates = new List<PlayerState>();
                // �ƴϸ� ���� ������
                else
                    receiveStates.Clear();

                // �÷��̾� ������ �����ϰ� ����Ʈ�� �߰�
                for (int i = 0, j = 0; i < receiveData.Length; i++)
                {
                    if (receiveData[i].playerName.Equals(playerName))
                        receivePlayerState = receiveData[i];
                    else
                        receiveStates.Add(receiveData[i]);
                }

            }
            catch (Exception e)
            {
                Debug.LogWarning(recvJson);
                Debug.LogError(e.StackTrace);
            }

            // ���Ź��� ��Ŷ�� ������ Ŭ���̾�Ʈ�� ����
            if (packet.Length > 0)
            {
                ApplyToClient();
            }
        }
    }

    /// <summary>
    /// TCP�� ���� �޽��� ������ ����
    /// </summary>
    /// <param name="message">���� �޽���</param>
    private void SetTcpSendPacket(string message)
    {
        sendMessage.playerName = playerName;
        sendMessage.message = message;
    }

    /// <summary>
    /// TCP �޽��� ����
    /// </summary>
    /// <param name="message">������ �޽���</param>
    public void SendTCP(string message)
    {
        try
        {
            // ��Ŷ ����
            SetTcpSendPacket(message);

            // Json���� ��ȯ(Serialize) �� byte array�� ��ȯ
            var sendMessageJson = JsonUtility.ToJson(sendMessage);
            var sendPacketBytes = Encoding.UTF8.GetBytes(sendMessageJson + "|");

            // �޽��� ����
            tcpClient.Send(sendPacketBytes, 0, sendPacketBytes.Length, SocketFlags.None);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }
    }

    /// <summary>
    /// TCP ������ ����
    /// </summary>
    private void ReceiveTcp()
    {
        int receive = 0;                        // ���Ź��� ������ ����
        byte[] packet = new byte[BUFFER_SIZE];  // ���� ���� ������ byte array
        // ���Ź��� �����Ͱ� ������
        if (tcpClient.Available != 0)
        {
            try
            {
                // �����͸� ���Ź޴´�.
                receive = tcpClient.Receive(packet);
                if (tcpDebug)
                    Debug.Log("[TCP]Received Size: " + receive);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            // ���� ���� �����͸� ���ڿ��� ��ȯ
            string recvJson = Encoding.UTF8.GetString(packet);
            if (tcpDebug)
                Debug.Log("Message: " + recvJson);

            // ������(|)�� �������� ���ڿ� �迭�� �ɰ���
            string[] jsons = recvJson.Split('|');

            // �� json �����Ϳ� ���� PlayerMessage�� ��ȯ(Deserialize)�ϰ�, �޽����� Ŭ���̾�Ʈ�� �����Ѵ�.
            for (int i = 0; i < jsons.Length; i++)
            {
                if (jsons[i].StartsWith("{"))
                {
                    try
                    {
                        Debug.Log("Convert json: " + jsons[i]);
                        receiveMessage = JsonUtility.FromJson<PlayerMessage>(jsons[i]);
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e.StackTrace);
                    } 

                    if (receive > 0)
                    {
                        ApplyMessage();
                    }
                }
            }

            if (tcpDebug)
                Debug.Log("Message: " + receiveMessage.message);
        }
       
    }

    /// <summary>
    /// 0.1�� �������� ���� Ƚ��(10ȸ)���ȸ� TCP ������ ������ ��ٸ���.
    /// </summary>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    IEnumerator ReceiveTcpRetry(int repeatCount = 0)
    {
        if(tcpDebug)
            Debug.Log("TCP Available: " + tcpClient.Available);
        // ���Ź��� �����Ͱ� ������ ���� ó��
        if (tcpClient.Available != 0)
        {
            yield return null;

            ReceiveTcp();
        }
        // ���Ź��� �����Ͱ� ������ ī�����ϰ� ���� �ڷ�ƾ ����
        else
        {
            yield return new WaitForSeconds(0.1f);
            repeatCount++;
            if (repeatCount < 10)
                StartCoroutine(ReceiveTcpRetry(repeatCount));
            else
                Debug.LogError("TCP Connection not available!");
        }
    }

    /// <summary>
    /// �÷��̾� �̸����� Ÿ �÷��̾� ���� ������Ʈ�� ã�´�
    /// </summary>
    /// <param name="name">ã�� �÷��̾� �̸�</param>
    /// <returns>Ÿ �÷��̾� ���� ������Ʈ</returns>
    private OtherPlayer GetOtherPlayerByName(string name)
    {
        foreach(OtherPlayer fobj in otherPlayers)
        {
            if (fobj.playerName.Equals(name))
                return fobj;
        }

        return null;
    }

    /// <summary>
    /// Ÿ �÷��̾ �ʵ忡 �߰� ��ȯ�Ѵ�.
    /// </summary>
    private void AddOtherPlayer()
    {
        for (int i = otherPlayers.Count; i < receiveStates.Count; i++)
        {
            otherPlayers.Add(Instantiate(otherPlayerSample, otherPlayerGroup));
            otherPlayers[i].gameObject.SetActive(true);
            otherPlayers[i].SetName(receiveStates[i].playerName);
        }
    }

    /// <summary>
    /// �޽����� �˻��Ͽ� Ŭ���̾�Ʈ�� �����Ѵ�
    /// </summary>
    private void ApplyMessage()
    {
        // ù handshaking �ܰ��� �޽����� ���
        if(receiveMessage.playerName.Equals("HANDSHAKING"))
        {
            // �޽����� Access ������ ���, UDP�� �ʱ�ȭ�Ͽ� �������� ����� �����Ѵ�
            string message = receiveMessage.message;
            if (message.Equals(MessageData.instance.Access.Accept) && udpClient == null)
            {
                InitUdpClient();
            }
        }
        // ���� �÷��̾ ���� �޽������� �˻�
        else if (receiveMessage.playerName.Equals(playerName))
        {
            string message = receiveMessage.message;
            // �޽����� Death�̸�, ���ó��
            if (message.Equals(MessageData.instance.Death))
            {
                player.Death();
            }
            //  �޽����� Respawn Accept�� ���, ������ ó��
            else if (message.Equals(MessageData.instance.Respawn.Accept))
            {
                player.Respawn();
            }
            // �޽����� Clear�� ���, Ŭ���� UI Ȱ��ȭ
            else if (message.Equals(MessageData.instance.Clear))
            {
                player.StageClear();
            }
        }
        else
        {
            Debug.LogWarning("Other's message received: " + receiveMessage.playerName);
        }
    }

    /// <summary>
    /// Ŭ���̾�Ʈ�� �÷��̾� ������ ����
    /// </summary>
    private void ApplyToClient()
    {
        // �� �÷��̾� ���� �̵��� �ʿ��ϸ�, ��ǥ�� ��� ����
        if(receivePlayerState.forceTransform)
            player.transform.position = new Vector3(receivePlayerState.positionX, receivePlayerState.positionY, receivePlayerState.positionZ);
        // �� �÷��̾� ���� ���� ����
        player.SetAlive(receivePlayerState.alive);

        // �߰� ������ �÷��̾ ������ �߰�
        if (otherPlayers.Count < receiveStates.Count)
            AddOtherPlayer();

        // �� Ÿ �÷��̾ ���� ��ġ/ȸ�� ���� �ݿ�
        for(int i = 0; i < receiveStates.Count; i++)
        {
            OtherPlayer otherPlayer = GetOtherPlayerByName(receiveStates[i].playerName);

            Vector3 pos = new Vector3(receiveStates[i].positionX, receiveStates[i].positionY, receiveStates[i].positionZ);
            Vector3 rot = new Vector3(receiveStates[i].rotationX, receiveStates[i].rotationY, receiveStates[i].rotationZ);
            otherPlayer.SetInfo(receiveStates[i].timestamp, pos, rot);
        }
    }

    /// <summary>
    /// ���� ���� ����
    /// </summary>
    private void CloseClient()
    {
        if(tcpClient != null) 
        {
            tcpClient.Close(); 
            tcpClient = null;
        }

        if(udpClient != null )
        {
            udpClient.Close();
            udpClient = null;
        }
    }
}
