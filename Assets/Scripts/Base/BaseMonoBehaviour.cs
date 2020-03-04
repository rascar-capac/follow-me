using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;

public class BaseMonoBehaviour : MonoBehaviour
{
    #region Pausing the game
    protected bool GamePaused = false;
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
    public void StartChrono(float seconds, Action callback = null, Action<float, float> FrameCallback = null) //Fonction sans return (void) - fonction avec return => Func<in, in, in, in, out>
    {
        StartCoroutine(Chronoing(seconds, callback, FrameCallback));
    }
    IEnumerator Chronoing(float seconds, Action callback, Action<float, float> FrameCallback = null)
    {
        float lasttime = 0.0f;
        while (true)
        {
            if (!GamePaused)
            {
                lasttime += Time.deltaTime;
                if (FrameCallback != null)
                    FrameCallback(lasttime, lasttime/seconds) ;
                if (lasttime >= seconds)
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
        if (GameManager.I)
            GameManager.I.onGamePaused.AddListener(GamePausedHandler);
    }
    #endregion Unity Standard events

    public virtual void Init() {; }
}

public class AnimationCallbackEvent : UnityEvent<string> { }
public class GamePausedEvent : UnityEvent<bool> { }