using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Data.SQLite;
using UnityEngine.Events;

public class BaseMonoBehaviour : MonoBehaviour
{
    #region Pausing the game
    public bool GamePaused = false;
    public virtual void GamePausedHandler(bool Paused)
    {
        GamePaused = Paused;
        Debug.Log($"Paused: {GamePaused}");
    }
    public GamePausedEvent onGamePaused;
    public void PauseGame(bool Pause)
    {
        onGamePaused.Invoke(Pause);
    }
    #endregion

    #region Indexer
    Dictionary<string, object> _properties = new Dictionary<string, object>();
    public object this[string name]
    {
        get
        {
            object o;
            if (_properties.TryGetValue(name, out o))
                return o;

            return null;
        }
        set 
        {
            if (this[name] == null)
                _properties.Add(name, value);
            else
                _properties[name] = value;
        }
    }
    #endregion

    #region Animations callback
    public AnimationCallbackEvent animCallback = new AnimationCallbackEvent();
    public void AnimationCallback(string animName)
    {
        animCallback?.Invoke(animName);
    }
    #endregion

    #region Chronometer
    float _lastTime = 0f;
    public void StartChrono(float seconds, Action callback = null) //Fonction sans return (void) - fonction avec return => Func<in, in, in, in, out>
    {
        _lastTime = 0f;
        StartCoroutine(Chronoing(seconds, callback));
    }
    IEnumerator Chronoing(float seconds, Action callback)
    {
        while (true)
        {
            if (!GamePaused)
            {
                _lastTime += Time.deltaTime;
                if (_lastTime >= seconds)
                    break;
            }
            yield return null;
        }
        if (callback != null)
            callback();
    }
    #endregion

    #region Unity Standard events
    protected virtual void Awake()
    {

    }
    protected virtual void Start()
    {
        GameManager.I.onGamePaused.AddListener(GamePausedHandler);
    }
    #endregion Unity Standard events

}

public class AnimationCallbackEvent : UnityEvent<string> { }
public class GamePausedEvent : UnityEvent<bool> { }