using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Universe : MonoBehaviour
{

    // Reference to the Prefab. Drag a Prefab into this field in the Inspector.
    public GameObject green_balloon;
    public GameObject red_balloon;
    public GameObject yellow_balloon;
    public GameObject blue_balloon;
    public GameObject pink_balloon;
    public GameObject cyan_balloon;
    public AudioClip pop;
    public AudioClip blop;
    public AudioClip fanfare;
    AudioSource audioSource;

    // Array of balloons
    GameObject[] balloons;

    // DR, DL, UR, UL, SR, SL
    float[] balloonsX = { 2f, -2.2f, 2f, -2.2f, 6.2f, -6.3f };
    float[] balloonsY = { -3.5f, -3.5f, 1.2f, 1.2f, -1.2f, -1.2f };


    int numberOfBalloonsOnScreen = 0;
    int numberOfBalloonsInTotal = 0;

    [SerializeField] private Game1 game1Data = new Game1();


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[6];

        audioSource = GetComponent<AudioSource>();

        Timer();

    }

    private float nextActionTime;

    

    void Timer()
    {
        foreach (GameObject balloon in balloons)
        {
            MoveBalloon(balloon);
        }

        if (Time.timeSinceLevelLoad > nextActionTime)
        {
            nextActionTime += Random.Range(Global.timeToNextBalloonMin, Global.timeToNextBalloonMax);

            if (numberOfBalloonsOnScreen < Global.maxNumberOfBalloonsOnScreen &&
                numberOfBalloonsInTotal < Global.maxNumberOfBalloonsInTotal)
            {
                InstantiateRandomPositionedBalloon();
            }

        }

        Invoke("Timer", Global.TimerInterval);
    }


    public void MoveBalloon(GameObject balloon)
    {

        if (balloon != null)
        {
            Vector3 p = balloon.transform.position;
            p.x = balloon.transform.position.x + Mathf.Sin(Time.time * Global.balloonAnimationSpeed) * Global.balloonAnimationDelta;
            p.y = balloon.transform.position.y + Mathf.Cos(Time.time * Global.balloonAnimationSpeed) * Global.balloonAnimationDelta;
            balloon.transform.position = p;
        }
    }

    public void InstantiateRandomPositionedBalloon()
    {
        numberOfBalloonsInTotal++;
        numberOfBalloonsOnScreen++;

        GameObject instantiatedBalloon = null;

        while (instantiatedBalloon == null)
        {
            int randomIndex = Random.Range(0, 6);
            if (balloons[randomIndex] == null)
            {
                balloons[randomIndex] = InstantiateRandomColoredBalloon(randomIndex);
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(int randomIndex)
    {
        int random = Random.Range(0, 6);
        Object original;

        switch (random)
        {
            case 0:
                original = green_balloon;
                break;
            case 1:
                original = red_balloon;
                break;
            case 2:
                original = yellow_balloon;
                break;
            case 3:
                original = blue_balloon;
                break;
            case 4:
                original = pink_balloon;
                break;
            case 5:
                original = cyan_balloon;
                break;
            default:
                original = green_balloon;
                break;
        }

        // Save the data of the generated balloon in data object
        game1Data.events.Add(new Event(Global.generateType, randomIndex, random));

        return InstantiateBalloon(original, balloonsX[randomIndex], balloonsY[randomIndex]);
    }

    public GameObject InstantiateBalloon(Object original, float positionX, float positionY)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, positionY, 0), Quaternion.identity) as GameObject;
    }

    public void OnMovement(string position)
    {
        //Debug.Log("OnMovement called: " + position);

        game1Data.messages.Add(position);

        switch (position)
        {
            case "DR":
                OnPop(0);
                break;

            case "DL":
                OnPop(1);
                break;

            case "UR":
                OnPop(2);
                break;

            case "UL":
                OnPop(3);
                break;

            case "SR":
                OnPop(4);
                break;

            case "SL":
                OnPop(5);
                break;

            default:
                break;
        }
    }

    public void OnPop(int index)
    {
        if (balloons[index] != null)
        {
            numberOfBalloonsOnScreen--;
            audioSource.PlayOneShot(pop, 0.7F);
            balloons[index].GetComponent<Animator>().enabled = true;
            Destroy(balloons[index], Global.popAnimationDuration);
            balloons[index] = null;

            // Save the data of the hit balloon in data object
            game1Data.events.Add(new Event(Global.hitType, index));

            IsGameFinished();
        }
    }

    public void IsGameFinished()
    {
        if (numberOfBalloonsInTotal == Global.maxNumberOfBalloonsInTotal)
        {
            bool IsAnyBalloonLeft = false;

            foreach (GameObject b in balloons)
            {
                if (b != null)
                {
                    IsAnyBalloonLeft = true;
                }

            }

            if (!IsAnyBalloonLeft && !AlreadyPlayedEnding)
            {
                PlayEnding();
                SaveIntoJson();
            }
        }
    }

    bool AlreadyPlayedEnding;

    public void PlayEnding()
    {

        AlreadyPlayedEnding = true;

        GameObject.Find("Targets").SetActive(false);

        Debug.Log("play ending");

        for (var i = 1; i < 8; i++)
        {
            GameObject.Find("p" + i.ToString()).GetComponentInChildren<ParticleSystem>().Play();
        }

        audioSource.PlayOneShot(fanfare, 1.0F);


    }

    public void SaveIntoJson()
    {
        Debug.Log("save button was clicked");

        string dataFolderPath = Application.persistentDataPath + "/Data/" + System.DateTime.Now.ToString("dd-MM-yyyy");

        if (!Directory.Exists(dataFolderPath))
        {
            //if it doesn't, create it
            Directory.CreateDirectory(dataFolderPath);

        }

        string data = JsonUtility.ToJson(game1Data);
        File.WriteAllText(dataFolderPath + "/Game_1_" + System.DateTime.Now.ToString("hh-mm-ss") + ".json", data);
    }

    void OnApplicationQuit()
    {
        SaveIntoJson();
    }

    [System.Serializable]
    public class Game1
    {
        public List<Event> events = new List<Event>();
        public List<string> messages = new List<string>();
    }

    [System.Serializable]
    public class Event
    {
        public string timestamp;
        public string type;
        public string position;
        public string color;

        public Event(string _type, int _position, int _color)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.targetPositions[_position];
            color = Global.colors[_color];
        }

        public Event(string _type, int _position)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.targetPositions[_position];
        }
    }
}

