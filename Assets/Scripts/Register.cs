using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Register : MonoBehaviour
{
    MongoClient client = new MongoClient("mongodb+srv://SENKOJIxBlockchain:senkojixblockchain@votes.e9rvz.mongodb.net/?retryWrites=true&w=majority");
    IMongoDatabase citizenDB;
    IMongoCollection<BsonDocument> citizenCollection;

    //public InputField nameInput;
    public InputField idInput;

    //private string register_name;
    public string register_id;

    public GameObject notfoundPanel;

    public object MessageBox { get; private set; }

    // Start is called before the first frame update
    void Start()
    {

        citizenDB = client.GetDatabase("CitizenDB");
        citizenCollection = citizenDB.GetCollection<BsonDocument>("CitizenCollection");

        // Insert Test
        //var document = new BsonDocument { { "citizen_id", "4444444444444" } };
        //citizenCollection.InsertOne(document);
    }

    // Update is called once per frame
    void Update()
    {
        //register_name = nameInput.text;
        register_id = idInput.text;
    }

    public void CheckCitizen()
    {
        var filter = Builders<BsonDocument>.Filter.Eq("citizen_id", register_id);
        var citizenDocument = citizenCollection.Find(filter).FirstOrDefault();
        if (citizenDocument == null)
        {
            Debug.Log("Not found citizen id");
            notfoundPanel.SetActive(true);
        }
        else
        {
            PlayerPrefs.SetString("user_id", register_id);
            Debug.Log(citizenDocument.ToString());
            SceneManager.LoadScene("Game");
        }
        //var element_name = "";
        //var element_value = "";
        //foreach (BsonElement element in citizenDocument)
        //{
        //    element_name = element.Name;
        //    element_value = element.Value.ToString();
        //}

    }

    public void ClosePanel()
    {
        notfoundPanel.SetActive(false);
    }
}
