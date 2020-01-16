using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : BaseMonoBehaviour
{
    public float PlayerLifeStart = 100.0f;
    public float PlayerLife = 100.0f;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        PlayerLife = PlayerLifeStart;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
