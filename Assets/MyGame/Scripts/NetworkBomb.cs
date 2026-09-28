using Mirror;
using UnityEngine;

public class NetworkBomb : NetworkBehaviour
{

    // 이 폭탄이 서버에 생성되면 Mirror가 호출된다.
    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log($"서버: 폭탄 생성, netId = {netId}");

        // 3초 후 DestroySelf 메서드를 실행한다.
        Invoke(nameof(DestroySelf), 3f);
    }

    // 이 메소드는 서버에서만 실행 가능하다.
    [Server]
    public void DestroySelf()
    {
        Debug.Log($"서버: 폭탄 제거, netId = {netId}");

        // 서버와 모든 Clinet에서 네트워크 오브젝트를 제거한다.
        NetworkServer.Destroy(gameObject);
    }
}
