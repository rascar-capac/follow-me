using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : Singleton<GameManager>
{
    protected override void Awake()
    {
        base.Awake();
        onGamePaused = new GamePausedEvent();
    }

    protected override void Start()
    {
        base.Start();
        //InputManager.I.onPauseKeyPressed.AddListener(() => { PauseGame(!GamePaused); });
    }
}


