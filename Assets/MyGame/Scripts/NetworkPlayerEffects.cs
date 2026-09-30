using System.Collections;
using Mirror;
using UnityEngine;

public class NetworkPlayerEffects : NetworkBehaviour
{
    [Header("효과")]
    [SerializeField] private float flashDuration = 0.5f;

    private SpriteRenderer playerRenderer;
    private Color defaultColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        playerRenderer = GetComponentInChildren<SpriteRenderer>();

        if (playerRenderer != null)
        {
            defaultColor = playerRenderer.color;
        }
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        defaultColor = Color.green;

        if (playerRenderer != null && flashRoutine == null)
        {
            playerRenderer.color = defaultColor;
        }

        Debug.Log($"내 플레이어입니다: netId = {netId}");
    }

    private void Update()
    {
        if (!isLocalPlayer) return;

        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdRequestGlobalEffect();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            CmdRequestPrivateEffect();
        }
    }

    [Command]
    private void CmdRequestGlobalEffect()
    {
        Debug.Log($"서버: 전체 효과 요청, netId = {netId}");
        RpcPlayGlobalEffect();
    }

    [ClientRpc]
    private void RpcPlayGlobalEffect()
    {
        PlayFlash(Color.red);
    }

    [Command]
    private void CmdRequestPrivateEffect()
    {
        Debug.Log($"서버: 개인 효과 요청, netId = {netId}");
        TargetPlayPrivateEffect(connectionToClient);
    }

    [TargetRpc]
    private void TargetPlayPrivateEffect(NetworkConnectionToClient target)
    {
        PlayFlash(Color.blue);
    }

    private void PlayFlash(Color flashColor)
    {
        if (playerRenderer == null) return;

        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
        }

        flashRoutine = StartCoroutine(FlashColor(flashColor));
    }

    private IEnumerator FlashColor(Color flashColor)
    {
        playerRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        playerRenderer.color = defaultColor;
        flashRoutine = null;
    }

    private void OnDisable()
    {
        if (flashRoutine != null)
        {
            StopCoroutine(flashRoutine);
            flashRoutine = null;
        }

        if (playerRenderer != null)
        {
            playerRenderer.color = defaultColor;
        }
    }
}
