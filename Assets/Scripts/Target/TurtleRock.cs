using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurtleRock : Zone
{
    PlayerInventory player;

    protected override void Start()
    {
        base.Start();
        player = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        //player.onQuestStoneFinished.AddListener(ElevateTurtleRock);
        transform.GetComponent<Collider>().enabled = false;
        ElevateTurtleRock();
    }

    public void ElevateTurtleRock()
    {
        StartCoroutine(Elevating());
    }

    IEnumerator Elevating()
    {
        while (true)
        {
            transform.position = Vector3.MoveTowards(transform.position, GameManager.I._data.TurtleRockFinalPosition.transform.position, 1 * Time.deltaTime);
            
            if (Vector3.Distance(transform.position, GameManager.I._data.TurtleRockFinalPosition.transform.position) <= 0.01f)
                break;

            yield return null;
        }
        transform.GetComponent<Collider>().enabled = true;
    }
}
