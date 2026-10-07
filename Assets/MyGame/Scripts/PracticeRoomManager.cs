using Mirror;
using UnityEngine;

public class PracticeRoomManager : NetworkRoomManager
{
    // 로비 관련 기능을 앞으로 여기서 확장한다.
    public override bool OnRoomServerSceneLoadedForPlayer(
    NetworkConnectionToClient conn,
    GameObject roomPlayer,
    GameObject gamePlayer)
    {
        // 1. roomPlayer에서 PracticeRoomPlayer 컴포넌트를 가져온다.
        PracticeRoomPlayer lobbyPlayer = roomPlayer.GetComponent<PracticeRoomPlayer>();
        // 2. gamePlayer에서 NetworkPlayerProfile 컴포넌트를 가져온다.
        NetworkPlayerProfile playerProfile = gamePlayer.GetComponent<NetworkPlayerProfile>();
        // 3. 둘 중 하나라도 null이면 오류 로그를 출력하고 false를 반환한다.
        if (lobbyPlayer == null || playerProfile == null)
            {
                Debug.Log("오류");
                return false;
            }

        // 4. 게임 플레이어의 SetPlayerName을 호출한다.
        //    인자로 로비 플레이어의 PlayerName을 전달한다.
        playerProfile.SetPlayerName(lobbyPlayer.PlayerName);

        // 5. 복사가 완료됐으므로 true를 반환한다.
        return true;
    }
}

