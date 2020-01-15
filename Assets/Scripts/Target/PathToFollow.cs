using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathToFollow : BaseMonoBehaviour
{
    Transform[] Checkpoints;

    void Awake()
    {
        Checkpoints = new Transform[gameObject.transform.childCount];
        for (int i = 0; i < gameObject.transform.childCount; i++)
        {
            Checkpoints[i] = gameObject.transform.GetChild(i);
        }
    }

    public IEnumerator<Transform> NextCheckPoint()
    {
        int index = 0;
        while (true)
        {
            yield return Checkpoints[(int)Mathf.Repeat(0, Checkpoints.Length)];
            index++;
        }
    }
}
