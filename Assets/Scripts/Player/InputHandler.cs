using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    enum GameState { PLAY, PAUSE, DIALOUGE}

    private PlayerController pc;
    private GameState state;
    // Start is called before the first frame update
    void Start()
    {
        pc = GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return)) 
        {
            pc.TrySpeak();
        }
        pc.MoveX(Input.GetAxisRaw("Horizontal"));
    }
}
