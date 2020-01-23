using UnityEngine;

public class Singleton<T> : BaseMonoBehaviour where T : BaseMonoBehaviour
{
    #region Variables
    private static T _I = null;
    public static T I
    {
        get
        {
            //if (_I == null)
            //    Debug.Assert(false, "No Instance of " + typeof(T));

            return _I;
        }
    }
    #endregion

    #region Unity functions
    protected override void Awake()
    {
        base.Awake();
        if (_I != this && _I != null)
        {
            DestroyImmediate(gameObject);
            return;
        }
        _I = this as T;
    }
    #endregion
}