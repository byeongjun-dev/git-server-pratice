using Mirror;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPlayerController : NetworkBehaviour
{
    private NetworkPlayerHealth playerHealth;

    [Header("이동")]
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D body;

    // 객체 초기화 시 실행되는 함수
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        // 같은 게임 오브젝트의 NetworkPlayerHealth 컴포넌트를 가져와
        playerHealth = gameObject.GetComponent<NetworkPlayerHealth>();
        // playerHealth에 저장한다.
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction =
            new Vector2(horizontal, vertical).normalized;

        body.linearVelocity = direction * moveSpeed;

        // playerHealth가 존재하고 IsDead가 true인지 확인한다.
        if (playerHealth != null && playerHealth.IsDead)
        {
            body.linearVelocity = Vector2.zero;
            return;
        }
    }
}
