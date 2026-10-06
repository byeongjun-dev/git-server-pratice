using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class NetworkPlayerLobby : NetworkBehaviour
{
    [SerializeField] private TMP_Text readyText;

    [SyncVar(hook = nameof(OnReadyChanged))]
    private bool isReady = false;

    public bool IsReady => isReady;

    public override void OnStartClient()
    {
        base.OnStartClient();

        UpdateReadyText();
    }

    private void Update()
    {
        // 1. 내 플레이어가 아니면 종료한다.
        if (!isLocalPlayer) return;

        // 2. Keyboard.current가 null이면 종료한다.
        if (Keyboard.current == null) return;

        // 3. F 키를 누른 순간인지 확인한다.
        //    힌트: Keyboard.current.fKey.wasPressedThisFrame
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            // 4. 서버에 준비 상태 변경을 요청한다.
            CmdToggleReady();
        }
    }

    [Command]
    private void CmdToggleReady()
    {
        // 서버에서 isReady를 반대 값으로 변경한다.
        isReady = !isReady;
        // false → true, true → false
        // 힌트: ! 연산자
    }

    private void OnReadyChanged(bool oldReady, bool newReady)
    {
        // 동기화된 준비 상태가 바뀌면 화면을 갱신한다.
        UpdateReadyText();
    }

    private void UpdateReadyText()
    {
        // 1. readyText가 null이면 종료한다.
        if (readyText == null) return;
        // 2. isReady가 true면 "준비 완료"를 표시한다.
        if (isReady == true) readyText.text = "Ready";
        // 3. false면 "준비 안 됨"을 표시한다.
        if (isReady == false) readyText.text = "Not Ready";
    }
}