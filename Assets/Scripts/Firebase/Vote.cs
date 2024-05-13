using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Firebase.Database;

public class Vote
{
    public string user_id;
    public int vote_number;
    public bool voted;

    public Vote()
    {

    }
    
    public Vote(string user_id, int vote_number, bool voted)
    {
        this.user_id = user_id;
        this.vote_number = vote_number;
        this.voted = voted;
    }

}
