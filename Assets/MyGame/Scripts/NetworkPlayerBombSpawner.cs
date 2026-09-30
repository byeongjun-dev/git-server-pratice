using Mirror;
using UnityEngine;

public class NetworkPlayerBombSpawner : NetworkBehaviour
{
    [Header("폭탄")]
    [SerializeField] private GameObject bombPrefab;
    [SerializeField] private float spawnDistance = 1.5f;
    [SerializeField] private float maxSpawnDistance = 3f;
    [SerializeField] private float bombCooldown = 2f;

    [SyncVar(hook = nameof(OnBombCountChanged))]
    private int remainingBombs = 3;

    private double nextBombAllowedTime;

    private void Update()
    {
        // 1. 내 플레이어가 아니면 아래 입력을 처리하지 않는다.
        if (!isLocalPlayer)
        {
            return;
        }
        // 2. B 키를 눌렀는지 확인한다.
        if (Input.GetKeyDown(KeyCode.B))
        {
            // 3. 플레이어의 오른쪽 spawnDistance 위치를 계산한다.
            Vector2 requestedPosition = (Vector2)transform.position + Vector2.right * spawnDistance;
            // 4. 계산한 위치를 전달하며 서버에 폭탄 생성을 요청한다.    
            CmdTrySpawnBomb(requestedPosition);    
        }
        // 5. Y 키를 눌렀는지 확인한다.
        if (Input.GetKeyDown(KeyCode.Y))
        {
            // 6. 서버의 거리 검증을 시험하기 위해
            //    플레이어로부터 오른쪽으로 100만큼 떨어진 위치를 계산한다.
            Vector2 invalidPosition = (Vector2)gameObject.transform.position + Vector2.right * 100f;

            // 7. 잘못된 위치를 전달하며 서버에 폭탄 생성을 요청한다.
            CmdTrySpawnBomb(invalidPosition);
        }
    }

    [Command]
    private void CmdTrySpawnBomb(Vector2 requestedPosition)
    {
        // 1. bombPrefab이 비어 있는지 검사한다.
        //    비어 있다면 요청한 클라이언트에게 거절 이유를 보내고 종료한다.
        if(bombPrefab == null)
        {
            TargetBombRejected(connectionToClient, "bombPrefab이 비어 있습니다.");
            return;
        }

        // 2. remainingBombs가 0 이하인지 검사한다.
        //    폭탄이 없다면 요청한 클라이언트에게 거절 이유를 보내고 종료한다.
        if (remainingBombs <= 0)
        {
            TargetBombRejected(connectionToClient, "bomb이 없습니다.");
            return;
        }

        // 3. 현재 서버 시간이 nextBombAllowedTime보다 이른지 검사한다.
        //    쿨타임 중이라면 요청한 클라이언트에게 거절 이유를 보내고 종료한다.
        if (NetworkTime.time < nextBombAllowedTime)
        {
            TargetBombRejected(connectionToClient, "bomb이 너무 빨리 스폰됩니다.");
            return;
        }

        // 4. 플레이어 위치와 requestedPosition 사이의 거리를 계산한다.
        float distance = Vector2.Distance(transform.position,requestedPosition);
        // 5. 계산한 거리가 maxSpawnDistance보다 큰지 검사한다.
        //    너무 멀다면 요청한 클라이언트에게 거절 이유를 보내고 종료한다.
        if (distance > maxSpawnDistance)
        {
            TargetBombRejected(connectionToClient, "폭탄 생성 거리를 초과했습니다.");
            return;
        }

        // 6. 모든 검사를 통과했으므로 remainingBombs를 1 감소시킨다.
        remainingBombs--;
        // 7. 현재 서버 시간에 bombCooldown을 더해서
        //    nextBombAllowedTime을 갱신한다.
        nextBombAllowedTime = NetworkTime.time + bombCooldown;
        // 8. requestedPosition에 bombPrefab을 생성한다.
        GameObject bomb = Instantiate(bombPrefab,requestedPosition,Quaternion.identity);        
        // 9. 생성된 폭탄을 NetworkServer.Spawn으로 등록한다.
        NetworkServer.Spawn(bomb);
        // 10. 서버 Console에 폭탄 생성 승인 기록을 출력한다.
        Debug.Log( $"서버 승인: 플레이어 {netId}가 " +  $"{requestedPosition} 위치에 폭탄 {bomb.name}을 생성했습니다.");
    }

    [TargetRpc]
    private void TargetBombRejected(NetworkConnectionToClient target, string reason)
    {
        Debug.Log($"폭탄 생성 거절: {reason}");
    }

    private void OnBombCountChanged(int oldCount, int newCount)
    {
        Debug.Log($"{oldCount} -> {newCount}");
    }
}