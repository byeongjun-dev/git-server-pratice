using Mirror;
using UnityEngine;

public class PracticeNetworkManager : NetworkManager
{
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("서버에 접속했습니다.");
    }

    public override void OnStartClient()
    {
        base.OnStartClient();   
        Debug.Log("클라이언트가 시작되었습니다.");
    }

    public override void OnServerConnect(NetworkConnectionToClient connection)
    {
        base.OnServerConnect(connection);
        Debug.Log($"서버: 클라이언트가 접속했습니다. ID = {connection.connectionId}");
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        Debug.Log("클라이언트: 서버 접속에 성공했습니다.");
    }
}