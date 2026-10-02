using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class NetworkPlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    private int health = 100;

    [SerializeField] 
    private TMP_Text nameText;

    // get 축약
    public bool IsDead => health <= 0;

    public override void OnStartClient()
{
    base.OnStartClient();

    if (nameText == null) return;
    nameText.text = $"Health: {health}";
}

    private void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }

        if (Keyboard.current == null)
        {
            return;
        } 

        if (Keyboard.current.hKey.wasPressedThisFrame)
        {
            CmdRequestPracticeDamage();
        }
    }

    [Command]
    private void CmdRequestPracticeDamage()
    {
        // 서버에서 TakeDamage를 호출하여 피해 20을 적용한다.
        TakeDamage(20);
        // 피해량은 서버 코드에서 정한다.
    }

    [Server]
    public void TakeDamage(int damage)
    {
        // 1. 체력이 이미 0 이하라면 종료한다.
        if (health <= 0)
        {
            return;
        }

        // 2. damage가 0 이하라면 종료한다.
        if (damage <= 0)
        {
            return;
        }

        // 3. 체력에서 damage를 뺀다.
        //    결과가 0 아래로 내려가지 않도록 제한한다.
        //    힌트: Mathf.Max(0, 계산한 체력)
        health = Mathf.Max(0, health - damage);

        // 4. 체력이 0이면 Die를 호출한다.
        if (health == 0)
        {
            Die();
        }
    }

    [Server]
    private void Die()
    {
        // 서버 Console에 사망한 플레이어의 netId를 출력한다.
        Debug.Log($"{netId}가 죽었습니다.");
        // 이번 단계에서는 플레이어 객체를 제거하지 않는다.
    }

    private void OnHealthChanged(int oldHealth, int newHealth)
    {
        // 클라이언트 Console에 이전 체력과 새 체력을 출력한다.
        if (nameText == null) return;
        nameText.text = $"Health: {newHealth}";
    }
}