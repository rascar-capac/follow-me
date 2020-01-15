using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPath : BaseMonoBehaviour
{
    public float UnitBySecond = 2f;
    public PathToFollow path;

    // Start is called before the first frame update
    void Start()
    {

        StartCoroutine(Follow());
    }

    IEnumerator Follow()
    {
        IEnumerator<Transform> point = path.NextCheckPoint();
        point.MoveNext();

        while (true)
        {
            if ((transform.position - point.Current.position).magnitude < 10f)
                point.MoveNext();

            transform.position = Vector3.MoveTowards(transform.position, point.Current.position, UnitBySecond * Time.deltaTime);
            yield return null;
        }
    }
}
