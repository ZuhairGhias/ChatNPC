using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using Utilities.Extensions;

public class PlayerController : MonoBehaviour
{
    public enum PlayerState { WALKING, WAITING, SPEAKING }
    public PlayerState State { get; private set; }

    [SerializeField]
    public bool FacingRight = true;

    [SerializeField]
    private Animator anim;

    [Header("Movement")]
    [SerializeField]
    [Range(1f, 2f)]
    public float moveSpeed = 2.0f;

    private Interactor interactor;
    private PlayerChatInput chatInput;
    private IInteractable npc;

    // Start is called before the first frame update
    void Start()
    {
        interactor = GetComponent<Interactor>();
        DebugUtils.HandleErrorIfNullGetComponent(interactor, this);

        chatInput = GetComponent<PlayerChatInput>();
        DebugUtils.HandleErrorIfNullGetComponent(chatInput, this);
        //PlayerChatInput.onEndEdit += SpeakToNpc;
    }

    private void OnDestroy()
    {
        //PlayerChatInput.onEndEdit -= SpeakToNpc;
    }

    // Update is called once per frame
    void Update()
    {
        interactor.Preview(FacingRight);
    }

    public void MoveX(float x)
    {
        if (State == PlayerState.WALKING)
        {
            if (x > 0)
            {
                FacingRight = true;
            }
            else if (x < 0)
            {
                FacingRight = false;
            }
            transform.position += Vector3.right * x * Time.deltaTime * moveSpeed;
            anim.SetFloat("Speed", x);
        }
        else
        {
            anim.SetFloat("Speed", 0);
        }

    }

    public void TrySpeak()
    {
        if(State == PlayerState.WALKING)
        {
            npc = interactor.GetInteractable(FacingRight);
            if (npc != null)
            {
                chatInput.EnableInput();
                SetPlayerState(PlayerState.SPEAKING);
            }
        }
        else if (State == PlayerState.SPEAKING)
        {
            SpeakToNpc(chatInput.CaptureInput());

        }
        
    }

    public void SpeakToNpc(string text)
    {
        npc.Interact(text);
        SetPlayerState(PlayerState.WALKING);
    }

    private void SetPlayerState(PlayerState state)
    {
        State = state;
    }
}
