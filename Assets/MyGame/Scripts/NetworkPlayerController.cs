using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class NetworkPlayerController : NetworkBehaviour
{
    [Header("화면 표시")]
    [SerializeField] private TMP_Text nameText;

    [Header("이동")]
    [SerializeField] private float moveSpeed = 5f;

    [Header("효과")]
    [SerializeField] private float flashDuration = 0.5f;

    [SyncVar(hook = nameof(OnScoreChanged))]
    private int score = 0;

    private Rigidbody2D body;
    private SpriteRenderer playerRenderer;
    private Color defaultColor;
    private Coroutine flashRoutine;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
        playerRenderer = GetComponentInChildren<SpriteRenderer>();

        if (playerRenderer != null)
        {
            defaultColor = playerRenderer.color;
        }
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        UpdatePlayerText();
        Debug.Log($"플레이어 생성: netId = {netId}");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        // 내 화면에서 내 플레이어의 기본 색상을 설정한다.
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

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdAddScore();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            CmdRequestGlobalEffect();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            CmdRequestPrivateEffect();
        }
    }

    private void FixedUpdate()
    {
        if (!isLocalPlayer) return;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction =
            new Vector2(horizontal, vertical).normalized;

        body.linearVelocity = direction * moveSpeed;
    }

    // 서버에서 점수를 변경한다.
    [Command]
    private void CmdAddScore()
    {
        score++;

        Debug.Log(
            $"서버 점수 증가: netId = {netId}, score = {score}"
        );
    }

    // 서버가 이 플레이어를 관찰 중인 클라이언트들에 효과를 보낸다.
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

    // 서버가 요청한 플레이어의 소유자에게만 효과를 보낸다.
    [Command]
    private void CmdRequestPrivateEffect()
    {
        Debug.Log($"서버: 개인 효과 요청, netId = {netId}");

        TargetPlayPrivateEffect(connectionToClient);
    }

    [TargetRpc]
    private void TargetPlayPrivateEffect(
        NetworkConnectionToClient target)
    {
        PlayFlash(Color.blue);
    }

    private void PlayFlash(Color flashColor)
    {
        if (playerRenderer == null) return;

        // 진행 중인 효과를 멈추고 새 효과를 시작한다.
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

    // 클라이언트의 점수가 갱신되면 이름표도 갱신한다.
    private void OnScoreChanged(int oldScore, int newScore)
    {
        UpdatePlayerText();

        Debug.Log(
            $"점수 변경: {oldScore} → {newScore}, netId = {netId}"
        );
    }

    private void UpdatePlayerText()
    {
        if (nameText == null) return;

        nameText.text = $"Player {netId}\nScore: {score}";
    }
}