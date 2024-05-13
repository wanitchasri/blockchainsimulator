using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Security.Cryptography;

public class VoteManager : MonoBehaviour
{

    //private string userID;

    public string user_id;
    public int vote_number;

    MongoClient client = new MongoClient("mongodb+srv://SENKOJIxBlockchain:senkojixblockchain@votes.e9rvz.mongodb.net/?retryWrites=true&w=majority");
    //IMongoDatabase citizenDB;
    IMongoDatabase voteDB;
    IMongoCollection<BsonDocument> voteCollection;

    public GameObject success_panel;

    // Start is called before the first frame update
    void Start()
    {
        //userID = SystemInfo.deviceUniqueIdentifier
        user_id = PlayerPrefs.GetString("user_id");

        voteDB = client.GetDatabase("VoteDB");
        voteCollection = voteDB.GetCollection<BsonDocument>("VoteCollection");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Vote1()
    {
        VoteCandidate(user_id, 1);
    }
    public void Vote2()
    {
        VoteCandidate(user_id, 2);
    }
    public void Vote3()
    {
        VoteCandidate(user_id, 3);
    }

    public void VoteCandidate(string user_id, int vote_number)
    {
        long time = new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds();
        string hashcode = sha256_hash(time.ToString());
        var newVote = new BsonDocument
            {
                { "hashcode", hashcode},
                { "timestamp", time },
                { "citizen_id", user_id },
                { "vote", vote_number },
                { "status", "waiting"}
                //{ "previous_hashcode", ""}
        };

        voteCollection.InsertOne(newVote);
        SucceedVote();
    }

    public void SucceedVote()
    {
        success_panel.SetActive(true);
    }

    public void ClosePanel()
    {
        SceneManager.LoadScene("Game");
    }
    public static String sha256_hash(String value)
    {
        StringBuilder Sb = new StringBuilder();

        using (SHA256 hash = SHA256Managed.Create())
        {
            Encoding enc = Encoding.UTF8;
            Byte[] result = hash.ComputeHash(enc.GetBytes(value));

            foreach (Byte b in result)
                Sb.Append(b.ToString("x2"));
        }

        return Sb.ToString();
    }
}
