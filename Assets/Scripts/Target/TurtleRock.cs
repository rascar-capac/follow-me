using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleRock : Zone
{
    public GameObject FinalPosition;

    PlayerInventory player;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        player.onQuestStoneFinished.AddListener(ElevateTurtleRock);
        transform.GetComponent<Collider>().enabled = false;
    }

    public void ElevateTurtleRock()
    {
        StartCoroutine(Elevating());
    }

    IEnumerator Elevating()
    {
        while (true)
        {
            Vector3.MoveTowards(transform.position, FinalPosition.transform.position, 1 * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, FinalPosition.transform.position) <= 0.01f)
                break;

            yield return null;
        }
        transform.GetComponent<Collider>().enabled = true;
    }
}
