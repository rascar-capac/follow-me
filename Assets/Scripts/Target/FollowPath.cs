using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : BaseMonoBehaviour
{
    [Header("Speed of the object that follow the path")]
    public float UnitBySecond = 2f;
    [Header("an empty gameobject with child. children are the checkpoints.")]
    public GameObject path;
    [Header("The distance threshold before following next checkpoint")]
    public float DistanceThreshold = 0.001f;

    Transform[] Checkpoints;
    int CheckPointIndex = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        EnumerateCheckPoints();
        StartCoroutine(Follow());
    }

    void EnumerateCheckPoints()
    {
        Checkpoints = new Transform[path.transform.childCount];
        for (int i = 0; i < path.transform.childCount; i++)
        {
            Checkpoints[i] = path.transform.GetChild(i);
        }
    }

    IEnumerator<Transform> NextCheckPoint()
    {
        CheckPointIndex = 0;
        while (true)
        {
            yield return Checkpoints[(int)Mathf.Repeat(CheckPointIndex, Checkpoints.Length)];
            CheckPointIndex++;
        }
    }

    IEnumerator Follow()
    {
        IEnumerator<Transform> point = NextCheckPoint();
        point.MoveNext();

        while (true)
        {
            if (Vector3.Distance(transform.position, point.Current.position) <= DistanceThreshold)
                point.MoveNext();

            transform.position = Vector3.MoveTowards(transform.position, point.Current.position, UnitBySecond * Time.deltaTime);
            yield return null;
        }
    }
}
