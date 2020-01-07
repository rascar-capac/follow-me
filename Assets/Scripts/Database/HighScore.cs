using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Data.SQLite;
using Dapper;
using System.Linq;

public class HighScore : BaseModel
{
    public string playerName { get; set; }
    public int score{ get; set; }

    public HighScore()
    {
        TableName = "HighScores";
    }
}
