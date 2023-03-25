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
/// 게임 서버와 직접 통신하는 클래스
/// </summary>
public class CommunicateManager : MonoBehaviour
{
    /// <summary>
    /// TCP 소켓 (메시지 통신용)
    /// </summary>
    private Socket tcpClient;
    /// <summary>
    /// UDP 소켓 (게임 데이터 통신용)
    /// </summary>
    private UdpClient udpClient;
    
    /// <summary>
    /// 게임 서버 IP
    /// </summary>
    private static string host = "172.30.1.49";
    /// <summary>
    /// TCP 통신 서버 포트 번호
    /// </summary>
    private static int tcpPort = 49990;
    /// <summary>
    /// UDP 통신 서버 포트 번호
    /// </summary>
    private static int udpPort = 49999;
    /// <summary>
    /// UDP 통신 클라이언트 포트 번호
    /// </summary>
    private static int udpSendPort = 49995;
    /// <summary>
    /// 최대 버퍼 크기
    /// </summary>
    private const int BUFFER_SIZE = 1024;

    /// <summary>
    /// 전송할 플레이어 데이터
    /// </summary>
    public PlayerState sendState = new PlayerState();
    /// <summary>
    /// 수신 받은 다른 플레이어 데이터
    /// </summary>
    public List<PlayerState> receiveStates;
    /// <summary>
    /// 수신 받은 자신에 대한 플레이어 데이터
    /// </summary>
    public PlayerState receivePlayerState;
    /// <summary>
    /// 전송할 TCP 메시지
    /// </summary>
    public PlayerMessage sendMessage;
    /// <summary>
    /// 전송 받은 TCP 메시지
    /// </summary>
    public PlayerMessage receiveMessage;

    /// <summary>
    /// TCP 서버 End point
    /// </summary>
    private IPEndPoint tcpEndpoint;
    /// <summary>
    /// UDP 서버 End point
    /// </summary>
    private IPEndPoint udpEndPoint;

    /// <summary>
    /// 플레이어 제어 오브젝트
    /// </summary>
    public Player player;
    /// <summary>
    /// 다른 플레이어 샘플 오브젝트
    /// </summary>
    public OtherPlayer otherPlayerSample;
    /// <summary>
    /// 소환된 다른 플레이어 오브젝트
    /// </summary>
    [SerializeField]
    private List<OtherPlayer> otherPlayers;
    /// <summary>
    /// 다른 플레이어의 소환 필드 Transform
    /// </summary>
    public Transform otherPlayerGroup;

    /// <summary>
    /// 플레이어 이름
    /// </summary>
    private static string playerName;
    /// <summary>
    /// (테스트용)포트번호 표시 텍스트
    /// </summary>
    public Text portText;

    /// <summary>
    /// UDP 관련 로그 표시 여부
    /// </summary>
    public bool udpDebug;
    /// <summary>
    /// TCP 관련 로그 표시 여부
    /// </summary>
    public bool tcpDebug;

    private void Awake()
    {
        // 메시지 데이터가 비어있으면 파일에서 읽어온다.
        if (MessageData.instance == null)
        {
            MessageData.instance = MessageData.ReadMessageDataFromFile();
        }
    }

    private void Start()
    {
        // 온라인 테스트이면, TCP 소켓과 타 플레이어 리스트 초기화한다
        if(!PlayManager.OfflineTest)
        {
            InitTcpClient();
            otherPlayers = new List<OtherPlayer>();
        }       
    }

    /// <summary>
    /// 주기적 통신 코루틴
    /// </summary>
    private IEnumerator Communicate()
    {
        // UDP로 플레이어 데이터를 송수신
        if (udpClient != null)
        {
            SendUDP();
            ReceiveUdp();
        }
        // TCP로 메시지 데이터 수신
        if(tcpClient != null)
            ReceiveTcp();

        // 0.1초 간격으로 반복
        yield return new WaitForSeconds(0.1f);

        StartCoroutine(Communicate());
    }

    /// <summary>
    /// 앱이 종료될 때 소켓을 닫는다.
    /// </summary>
    private void OnApplicationQuit()
    {
        CloseClient();
    }

    /// <summary>
    /// TCP 소켓 초기화 작업
    /// </summary>
    private void InitTcpClient()
    {
        // TCP IP endpoint를 초기화하고, TCP 클라이언트를 TCP 세팅으로 초기화
        tcpEndpoint = new IPEndPoint(IPAddress.Parse(host), tcpPort);
        tcpClient = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        // 서버에 연결 요청
        tcpClient.Connect(tcpEndpoint);

        // TCP 수신 대기
        StartCoroutine(ReceiveTcpRetry());

        // 플레이어 정보 전송
        SendTCP(playerName);
        // Non-blocking TCP socket으로 설정
        tcpClient.Blocking = false;
    }

    /// <summary>
    /// UDP 소켓 초기화 작업
    /// </summary>
    private void InitUdpClient()
    {
        // UDP IP endpoint 및 소켓 초기화
        udpEndPoint = new IPEndPoint(IPAddress.Parse(host), udpPort);
        udpClient = new UdpClient(udpSendPort);

        // 주기적 통신 시작
        StartCoroutine(Communicate());
    }

    /// <summary>
    /// 서버에 전송할 플레이어 이름 전송
    /// </summary>
    /// <param name="name">플레이어 이름</param>
    public void SetPlayerName(string name)
    {
        playerName = name;
    }

    /// <summary>
    /// UDP 클라이언트의 포트 번호 지정
    /// </summary>
    /// <param name="port">클라이언트 UDP 포트 번호</param>
    public void SetUdpPort(int port)
    {
        udpSendPort = port;
    }

    /// <summary>
    /// UDP로 송신할 플레이어 데이터 설정
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
    /// UDP 플레이어 데이터 전송
    /// </summary>
    private void SendUDP()
    {
        try
        {
            // 데이터 복사
            SetUdpSendPacket();

            // Json/byte array로 변환하여 UDP 서버로 데이터 전송
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
    /// UDP 플레이어 데이터 수신
    /// </summary>
    private void ReceiveUdp()
    {
        if(udpDebug)
            Debug.Log("UDP Client Available: " + udpClient.Available);

        // 버퍼에 수신할 데이터가 있을 때
        if (udpClient.Available != 0)
        {
            // 클라이언트의 포트 번호 정보 UI 업데이트
            string port = ((IPEndPoint)udpClient.Client.LocalEndPoint).Port.ToString();
            portText.text = "Port: " + port;
            // 수신받을 데이터 배열 초기화
            byte[] packet = new byte[1024];
            try
            {
                // 데이터 수신
                packet = udpClient.Receive(ref udpEndPoint);
                if(udpDebug)
                    Debug.Log("[Receive] " + udpEndPoint.ToString() + "로부터 " + packet.Length + "byte 수신");
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
                return;
            }

            // 수신받은 byte array를 문자열로 변환
            string recvJson = Encoding.UTF8.GetString(packet);

            try
            {
                // Json 데이터를 PlayerState 클래스로 변환 (Deserialize)
                PlayerState[] receiveData = JsonHelper.FromJson<PlayerState>(recvJson);
                // 수신받을 리스트가 초기화되지 않았으면 초기화
                if (receiveStates == null)
                    receiveStates = new List<PlayerState>();
                // 아니면 내용 날리기
                else
                    receiveStates.Clear();

                // 플레이어 본인을 제외하고 리스트에 추가
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

            // 수신받은 패킷이 있으면 클라이언트에 적용
            if (packet.Length > 0)
            {
                ApplyToClient();
            }
        }
    }

    /// <summary>
    /// TCP로 보낼 메시지 데이터 세팅
    /// </summary>
    /// <param name="message">보낼 메시지</param>
    private void SetTcpSendPacket(string message)
    {
        sendMessage.playerName = playerName;
        sendMessage.message = message;
    }

    /// <summary>
    /// TCP 메시지 전송
    /// </summary>
    /// <param name="message">전송할 메시지</param>
    public void SendTCP(string message)
    {
        try
        {
            // 패킷 설정
            SetTcpSendPacket(message);

            // Json으로 변환(Serialize) 후 byte array로 변환
            var sendMessageJson = JsonUtility.ToJson(sendMessage);
            var sendPacketBytes = Encoding.UTF8.GetBytes(sendMessageJson + "|");

            // 메시지 전송
            tcpClient.Send(sendPacketBytes, 0, sendPacketBytes.Length, SocketFlags.None);
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
            return;
        }
    }

    /// <summary>
    /// TCP 데이터 수신
    /// </summary>
    private void ReceiveTcp()
    {
        int receive = 0;                        // 수신받은 데이터 길이
        byte[] packet = new byte[BUFFER_SIZE];  // 수신 받은 데이터 byte array
        // 수신받을 데이터가 있으면
        if (tcpClient.Available != 0)
        {
            try
            {
                // 데이터를 수신받는다.
                receive = tcpClient.Receive(packet);
                if (tcpDebug)
                    Debug.Log("[TCP]Received Size: " + receive);
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }

            // 수신 받은 데이터를 문자열로 변환
            string recvJson = Encoding.UTF8.GetString(packet);
            if (tcpDebug)
                Debug.Log("Message: " + recvJson);

            // 구분자(|)를 기준으로 문자열 배열로 쪼갠다
            string[] jsons = recvJson.Split('|');

            // 각 json 데이터에 대해 PlayerMessage로 변환(Deserialize)하고, 메시지를 클라이언트에 적용한다.
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
    /// 0.1초 간격으로 일정 횟수(10회)동안만 TCP 데이터 수신을 기다린다.
    /// </summary>
    /// <param name="repeatCount"></param>
    /// <returns></returns>
    IEnumerator ReceiveTcpRetry(int repeatCount = 0)
    {
        if(tcpDebug)
            Debug.Log("TCP Available: " + tcpClient.Available);
        // 수신받을 데이터가 있으면 수신 처리
        if (tcpClient.Available != 0)
        {
            yield return null;

            ReceiveTcp();
        }
        // 수신받을 데이터가 없으면 카운팅하고 다음 코루틴 실행
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
    /// 플레이어 이름으로 타 플레이어 제어 오브젝트를 찾는다
    /// </summary>
    /// <param name="name">찾을 플레이어 이름</param>
    /// <returns>타 플레이어 제어 오브젝트</returns>
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
    /// 타 플레이어를 필드에 추가 소환한다.
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
    /// 메시지를 검사하여 클라이언트에 적용한다
    /// </summary>
    private void ApplyMessage()
    {
        // 첫 handshaking 단계의 메시지인 경우
        if(receiveMessage.playerName.Equals("HANDSHAKING"))
        {
            // 메시지가 Access 수락인 경우, UDP를 초기화하여 본격적인 통신을 시작한다
            string message = receiveMessage.message;
            if (message.Equals(MessageData.instance.Access.Accept) && udpClient == null)
            {
                InitUdpClient();
            }
        }
        // 현재 플레이어에 대한 메시지인지 검사
        else if (receiveMessage.playerName.Equals(playerName))
        {
            string message = receiveMessage.message;
            // 메시지가 Death이면, 사망처리
            if (message.Equals(MessageData.instance.Death))
            {
                player.Death();
            }
            //  메시지가 Respawn Accept인 경우, 리스폰 처리
            else if (message.Equals(MessageData.instance.Respawn.Accept))
            {
                player.Respawn();
            }
            // 메시지가 Clear인 경우, 클리어 UI 활성화
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
    /// 클라이언트에 플레이어 데이터 적용
    /// </summary>
    private void ApplyToClient()
    {
        // 본 플레이어 강제 이동이 필요하면, 좌표값 즉시 적용
        if(receivePlayerState.forceTransform)
            player.transform.position = new Vector3(receivePlayerState.positionX, receivePlayerState.positionY, receivePlayerState.positionZ);
        // 본 플레이어 생존 여부 설정
        player.SetAlive(receivePlayerState.alive);

        // 추가 접속한 플레이어가 있으면 추가
        if (otherPlayers.Count < receiveStates.Count)
            AddOtherPlayer();

        // 각 타 플레이어에 대한 위치/회전 정보 반영
        for(int i = 0; i < receiveStates.Count; i++)
        {
            OtherPlayer otherPlayer = GetOtherPlayerByName(receiveStates[i].playerName);

            Vector3 pos = new Vector3(receiveStates[i].positionX, receiveStates[i].positionY, receiveStates[i].positionZ);
            Vector3 rot = new Vector3(receiveStates[i].rotationX, receiveStates[i].rotationY, receiveStates[i].rotationZ);
            otherPlayer.SetInfo(receiveStates[i].timestamp, pos, rot);
        }
    }

    /// <summary>
    /// 서버 접속 종료
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
