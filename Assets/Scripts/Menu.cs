using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class Menu : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://SENKOJIxBlockchain:senkojixblockchain@votes.e9rvz.mongodb.net/?retryWrites=true&w=majority");
    IMongoDatabase citizenDB;
    IMongoCollection<BsonDocument> citizenCollection;
    IMongoDatabase candidateDB;
    IMongoCollection<BsonDocument> candidateCollection;
    IMongoDatabase voteDB;
    IMongoCollection<BsonDocument> voteCollection;

    public Text credit_text;
    int credit = 0;

    public InputField search_input;

    public GameObject candidatePanel;
    public GameObject resultChoicePanel;
    public GameObject succeededPanel;
    public GameObject failedPanel;
    public GameObject waitingPanel;
    public GameObject searchPanel;

    public Text candidate_content;
    public Text succeeded_content;
    public Text failed_content;
    public Text waiting_content;
    public Text search_content;

    public GameObject validatePanel;
    public GameObject processingPanel;
    public GameObject validateResultPanel;
    public Text validate_content;

    // Start is called before the first frame update
    void Start()
    {

        citizenDB = client.GetDatabase("CitizenDB");
        citizenCollection = citizenDB.GetCollection<BsonDocument>("CitizenCollection");
        candidateDB = client.GetDatabase("CandidateDB");
        candidateCollection = candidateDB.GetCollection<BsonDocument>("CandidateCollection");
        voteDB = client.GetDatabase("VoteDB");
        voteCollection = voteDB.GetCollection<BsonDocument>("VoteCollection");

        // Insert Test
        //var document = new BsonDocument { { "citizen_id", "4444444444444" } };
        //citizenCollection.InsertOne(document);
    }

    // Update is called once per frame
    void Update()
    {
        ShowCredit();
    }

    public async void ShowCandidate()
    {
        candidatePanel.SetActive(true);

        var candidateDocument = candidateCollection.FindAsync(new BsonDocument());
        var candidateAwaited = await candidateDocument;
        var output = "";

        foreach (BsonDocument doc in candidateAwaited.ToList())
        {
            foreach (BsonElement candidate in doc)
            {
                if (candidate.Name == "number")
                {
                    output += "No: " + candidate.Value;
                }
                if (candidate.Name == "name")
                {
                    output += "  " + candidate.Value;
                }
            }
            output += "\n\n";
        }
        candidate_content.text = output;
    }

    public void ValidateVote()
    {
        validatePanel.SetActive(true);
        processingPanel.SetActive(true);

        string user_id = PlayerPrefs.GetString("user_id");

        string idToValidate = "";
        string validate_result = "";

        var statusFilter = Builders<BsonDocument>.Filter.Eq("status", "waiting");
        var voteDocument = voteCollection.Find(statusFilter);

        ObjectId _id = new ObjectId();

        if (voteDocument.ToList().Count != 0)
        {
            foreach (BsonElement vote in voteDocument.FirstOrDefault())
            {
                if (vote.Name == "_id")
                {
                    _id = vote.Value.AsObjectId;
                }
                //Debug.Log(voteDocument.FirstOrDefault().ToString());
                if (vote.Name == "citizen_id")
                {
                    idToValidate = vote.Value.ToString();
                }
            }

            var idFilter = Builders<BsonDocument>.Filter.Eq("citizen_id", idToValidate);
            var userDocument = citizenCollection.Find(idFilter).FirstOrDefault();
            bool voteExisted = false;

            foreach (BsonElement user in userDocument)
            {
                if (user.Name == "voted")
                {
                    if (user.Value == false)
                    {
                        var votedUpdate = Builders<BsonDocument>.Update.Set("voted", true);
                        citizenCollection.UpdateOne(idFilter, votedUpdate);
                        voteExisted = false;
                    } else
                    {
                        voteExisted = true;
                    }
                }
            }

            var hashFilter = Builders<BsonDocument>.Filter.Eq("_id", _id);
            if (voteExisted)
            {
                var statusUpdate = Builders<BsonDocument>.Update.Set("status", "failed");
                voteCollection.UpdateOne(hashFilter, statusUpdate);
                validate_result = "Successfully Validated! \nFound invalid vote :(";
            } else
            {
                var statusUpdate = Builders<BsonDocument>.Update.Set("status", "success");
                voteCollection.UpdateOne(hashFilter, statusUpdate);
                validate_result = "Successfully Validated! \nValid vote :D";
            }
            var userIdFilter = Builders<BsonDocument>.Filter.Eq("citizen_id", user_id);
            var creditUpdate = Builders<BsonDocument>.Update.Set("credit", credit + 1);
            citizenCollection.UpdateOne(userIdFilter, creditUpdate);

        } else
        {
            validate_result = "Sorry.. \nNo more vote to validate..";
        }
        processingPanel.SetActive(false);
        validateResultPanel.SetActive(true);
        validate_content.text = validate_result;
    }

    public void ShowCredit()
    {
        string user_id = PlayerPrefs.GetString("user_id");
        //Debug.Log(user_id);
        var filter = Builders<BsonDocument>.Filter.Eq("citizen_id", user_id);
        var userDocument = citizenCollection.Find(filter).FirstOrDefault();

        foreach (BsonElement user in userDocument)
        {
            if (user.Name == "credit")
            {
                credit = user.Value.ToInt32();
            }
        }
        credit_text.text = credit.ToString();
    }

    public void GoToVote()
    {
        SceneManager.LoadScene("Vote");
    }

    public void CloseCandidatePanel()
    {
        candidatePanel.SetActive(false);
    }

    public void OpenResultChoice()
    {
        resultChoicePanel.SetActive(true);
    }

    public async void OpenSucceeded()
    {
        CloseResultChoice();
        succeededPanel.SetActive(true);

        var filter = Builders<BsonDocument>.Filter.Eq("status", "success");
        var voteDocument = voteCollection.FindAsync(filter);
        var voteAwaited = await voteDocument;
        var output = "";

        foreach (BsonDocument doc in voteAwaited.ToList())
        {
            foreach (BsonElement vote in doc)
            {
                if (vote.Name == "timestamp")
                {
                    output += "Timestamp: " + vote.Value;
                }
                if (vote.Name == "vote")
                {
                    output += "  Vote: " + vote.Value;
                }
            }
            output += "\n\n";
        }
        succeeded_content.text = output;
    }

    public async void OpenFailed()
    {
        CloseResultChoice();
        failedPanel.SetActive(true);

        var filter = Builders<BsonDocument>.Filter.Eq("status", "failed");
        var voteDocument = voteCollection.FindAsync(filter);
        var voteAwaited = await voteDocument;
        var output = "";

        foreach (BsonDocument doc in voteAwaited.ToList())
        {
            foreach (BsonElement vote in doc)
            {
                if (vote.Name == "timestamp")
                {
                    output += "Timestamp: " + vote.Value;
                }
                if (vote.Name == "vote")
                {
                    output += "  Vote: " + vote.Value;
                }
            }
            output += "\n\n";
        }
        failed_content.text = output;
    }

    public async void OpenWaiting()
    {
        CloseResultChoice();
        waitingPanel.SetActive(true);

        var filter = Builders<BsonDocument>.Filter.Eq("status", "waiting");
        var voteDocument = voteCollection.FindAsync(filter);
        var voteAwaited = await voteDocument;
        var output = "";

        foreach (BsonDocument doc in voteAwaited.ToList())
        {
            foreach (BsonElement vote in doc)
            {
                if (vote.Name == "timestamp")
                {
                    output += "Timestamp: " + vote.Value;
                }
                if (vote.Name == "vote")
                {
                    output += "  Vote: " + vote.Value;
                }
            }
            output += "\n\n";
        }
        waiting_content.text = output;
    }

    public void Search()
    {
        CloseResultChoice();
        searchPanel.SetActive(true);

        int search = Convert.ToInt32(search_input.text);

        var filter = Builders<BsonDocument>.Filter.Eq("vote", search);
        var voteDocument = voteCollection.Find(filter).ToList();
        // var voteAwaited = voteDocument;
        var output = "";

        if (voteDocument.Count != 0)
        {
            foreach (BsonDocument doc in voteDocument)
            {
                foreach (BsonElement vote in doc)
                {
                    if (vote.Name == "timestamp")
                    {
                        output += "Timestamp: " + vote.Value;
                    }
                    if (vote.Name == "vote")
                    {
                        output += "  Vote: " + vote.Value;
                    }
                }
                output += "\n\n";
            }
            search_content.text = output;
        }
        else
        {
            search_content.text = "No result";
        }
    }

    public void CloseResultChoice()
    {
        resultChoicePanel.SetActive(false);
    }

    public void CloseSucceededPanel()
    {
        succeededPanel.SetActive(false);
    }

    public void CloseFailedPanel()
    {
        failedPanel.SetActive(false);
    }

    public void CloseWaitingPanel()
    {
        waitingPanel.SetActive(false);
    }

    public void CloseSearchPanel()
    {
        searchPanel.SetActive(false);
    }

    public void CloseValidatePanel()
    {
        validatePanel.SetActive(false);
        validateResultPanel.SetActive(false);
    }

    public void Leave()
    {
        SceneManager.LoadScene("Lobby");
    }


}
