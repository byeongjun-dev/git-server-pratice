using Mirror;
using UnityEngine;

public class NetworkPlayerProfile : NetworkBehaviour
{
    [SyncVar]
    private string playerName;

    public string PlayerName => playerName;

    [Server]
    public void SetPlayerName(string newName)
    {
        // 1. 전달받은 newName을 playerName에 저장한다.
        playerName = newName;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        // 2. 게임 캐릭터의 이름과 netId를 Console에 출력한다.
        Debug.Log($"{playerName}, {netId}");
    }
}