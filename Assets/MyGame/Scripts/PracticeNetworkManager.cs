using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

public class PracticeNetworkManager : NetworkManager
{
    [SerializeField] private string gameSceneName = "GameScene";

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

    private void Update()
    {
        // 1. 서버가 실행 중이 아니면 종료한다.
        // 힌트: NetworkServer.active
        if (NetworkServer.active == false) return;

        // 2. 로컬 클라이언트가 실행 중이 아니면 종료한다.
        // 힌트: NetworkClient.active
        // 두 조건을 함께 사용하면 이번 실습에서는 Host에서 처리한다.
        if (NetworkClient.active == false) return;

        // 3. Keyboard.current가 null이면 종료한다
        if (Keyboard.current == null) return;
        
        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            if (CanStartGame())
            {
                Debug.Log("Server: All players are ready. Game can start!");
                TryStartGame();
            }
            else
            {
                Debug.Log("Server: At least 2 players must connect and all must be ready.");
            }
        }

        if (Keyboard.current.enterKey.wasPressedThisFrame)
        {
            TryStartGame();
        }
    }

    [Server]
    private void TryStartGame()
    {
        // 1. CanStartGame()이 false이면
        //    준비가 부족하다는 로그를 출력하고 종료한다.
        if (CanStartGame() == false)
        {
            Debug.Log("Not ready");
            return;
        }

        // 2. 이미 게임 씬이라면 종료한다.
        //    힌트: networkSceneName == gameSceneName
        if (networkSceneName == gameSceneName)
        {
            return;
        } 
            // 3. 게임 씬으로 이동한다는 로그를 출력한다.
            Debug.Log("go");
            
            // 4. 서버와 연결된 모든 클라이언트의 씬을 변경한다.
            // 힌트: ServerChangeScene(gameSceneName);
            ServerChangeScene(gameSceneName);
    }

        public bool CanStartGame()
    {
        // 1. NetworkServer.active가 false이면 false를 반환한다.
        // 서버가 실행 중일 때만 검사한다.
        if (NetworkServer.active == false) return false;

        // 2. NetworkServer.connections.Count가 2보다 작으면
        // false를 반환한다.
        if (NetworkServer.connections.Count < 2) return false;

        // 3. 아래 반복문으로 각 접속자를 검사한다.
        foreach (NetworkConnectionToClient connection
                in NetworkServer.connections.Values)
        {
            // 4. connection.identity가 null이면 false를 반환한다.
            // 접속했지만 플레이어 객체가 아직 없을 수 있다.
            if (connection.identity == null) return false;

            // 5. connection.identity에서
            // NetworkPlayerLobby 컴포넌트를 가져온다.
            // NetworkPlayerLobby 타입의 변수에 저장한다.
            // 힌트: GetComponent<NetworkPlayerLobby>()
            NetworkPlayerLobby lobbyPlayer = connection.identity.GetComponent<NetworkPlayerLobby>();

            // 6. 가져온 컴포넌트가 null이면 false를 반환한다.
            if (lobbyPlayer == null) return false;

            // 7. 해당 플레이어의 IsReady가 false이면
            // false를 반환한다.
            if (lobbyPlayer.IsReady == false) return false;
        }

        // 8. 모든 검사를 통과했다면 true를 반환한다.
        return true;
    }
}