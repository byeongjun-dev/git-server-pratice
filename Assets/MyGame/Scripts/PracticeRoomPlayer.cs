using Mirror;
using UnityEngine;

public class PracticeRoomPlayer : NetworkRoomPlayer
{
    // 서버에서 정한 이름을 클라이언트에 공유한다.
    [SyncVar]
    private string playerName;

    // 다른 스크립트에서 이름을 읽을 수 있게 한다.
    public string PlayerName => playerName;

    public override void OnStartServer()
    {
        base.OnStartServer();

        // 1. playerName에 "Player "와 netId를 조합해서 저장한다.
        //    예: Player 1
        //    힌트: 문자열 보간 $"..."
        playerName = $"Player{netId}";

        // 2. 서버 Console에 지정한 이름을 출력한다.
        Debug.Log(playerName);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // 3. 클라이언트 Console에 받은 playerName을 출력한다.
        Debug.Log(playerName);
    }
}