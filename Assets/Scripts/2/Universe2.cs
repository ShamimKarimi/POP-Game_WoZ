using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Universe2 : MonoBehaviour
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

    // UI
    GameObject nextSceneButton;

    // Array of balloons
    GameObject[] balloons;

    // left to right
    float[] balloonsX = { -6.3f, -2.2f, 2f, 6.2f, -6.3f, -2.2f, 2f, 6.2f };

    int numberOfBalloonsInTotal = 0;

    [SerializeField] private Game2 game2Data = new Game2();


    // Start is called before the first frame update
    void Start()
    {

        balloons = new GameObject[8];

        audioSource = GetComponent<AudioSource>();

        Timer();
    }

    private float nextActionTime;

    void KeyEvent()
    {
        if (Input.GetKeyDown("k"))
            OnMovement("SR");
        if (Input.GetKeyDown("g"))
            OnMovement("SL");
        if (Input.GetKeyDown("u"))
            OnMovement("UR");
        if (Input.GetKeyDown("y"))
            OnMovement("UL");
        if (Input.GetKeyDown("n"))
            OnMovement("DR");
        if (Input.GetKeyDown("b"))
            OnMovement("DL");

    }

    void Timer()
    {

        KeyEvent();


        for (int i = 0; i < balloons.Length; i++)
        {
            MoveBalloon(balloons[i], i);
        }

        if (Time.timeSinceLevelLoad > nextActionTime)
        {

            nextActionTime += Random.Range(Global.timeToNextBalloonMin, Global.timeToNextBalloonMax);

            if (numberOfBalloonsInTotal < Global.maxNumberOfBalloonsInTotal)
            {
                InstantiateRandomPositionedBalloon();
            }

        }

        Invoke("Timer", Global.TimerInterval);
    }

    public void MoveBalloon(GameObject balloon, int index)
    {

        if (balloon != null)
        {
            Vector3 p = balloon.transform.position;

            // not popped, out of screen
            if (p.y > 9)
            {
                balloons[index] = null;
                Destroy(balloon);

                IsGameFinished();
            }
            // moving up, visible on the screen
            else
            {
                p.x = balloon.transform.position.x + Mathf.Sin(Time.time * Global.balloonAnimationSpeed) * Global.balloonAnimationDelta;
                p.y = balloon.transform.position.y + Global.balloonVerticalTranslationDelta;
                balloon.transform.position = p;
            }
        }
    }


    public void InstantiateRandomPositionedBalloon()
    {
        numberOfBalloonsInTotal++;

        GameObject instantiatedBalloon = null;

        while (instantiatedBalloon == null)
        {
            int randomIndex = Random.Range(0, 8);
            if (balloons[randomIndex] == null)
            {
                balloons[randomIndex] = InstantiateRandomColoredBalloon(randomIndex);
                instantiatedBalloon = balloons[randomIndex];
            }
        }

    }

    public GameObject InstantiateRandomColoredBalloon(int position)
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
        game2Data.events.Add(new Event(Global.generateType, position, random));

        return InstantiateBalloon(original, balloonsX[position]);
    }

    public GameObject InstantiateBalloon(Object original, float positionX)
    {
        audioSource.PlayOneShot(blop, 0.7F);
        return Instantiate(original, new Vector3(positionX, -5, 0), Quaternion.identity) as GameObject;
    }

    public void OnMovement(string position)
    {
        //Debug.Log("OnMovement called: " + position);

        game2Data.messages.Add(position);

        float[] yLevelMin = { -6.35f, -3.85f, -1.35f };
        float[] yLevelMax = { -2f, 0.5f, 3f };

        switch (position)
        {
            case "DR":
                OnPop(2, yLevelMin[0], yLevelMax[0], position);
                OnPop(6, yLevelMin[0], yLevelMax[0], position);
                break;

            case "DL":
                OnPop(1, yLevelMin[0], yLevelMax[0], position);
                OnPop(5, yLevelMin[0], yLevelMax[0], position);
                break;

            case "UR":
                OnPop(2, yLevelMin[2], yLevelMax[2], position);
                OnPop(6, yLevelMin[2], yLevelMax[2], position);
                break;

            case "UL":
                OnPop(1, yLevelMin[2], yLevelMax[2], position);
                OnPop(5, yLevelMin[2], yLevelMax[2], position);
                break;

            case "SR":
                OnPop(3, yLevelMin[1], yLevelMax[1], position);
                OnPop(7, yLevelMin[1], yLevelMax[1], position);
                break;

            case "SL":
                OnPop(0, yLevelMin[1], yLevelMax[1], position);
                OnPop(4, yLevelMin[1], yLevelMax[1], position);
                break;
        }
    }

    public void OnPop(int index, float yMin, float yMax, string position)
    {
        if (balloons[index] != null)
        {
            Vector3 p = balloons[index].transform.position;
            if (p.y > yMin && p.y < yMax)
            {
                audioSource.PlayOneShot(pop, 0.7F);
                balloons[index].GetComponent<Animator>().enabled = true;
                Destroy(balloons[index], Global.popAnimationDuration);
                balloons[index] = null;

                // Save the data of the hit balloon in data object
                game2Data.events.Add(new Event(Global.hitType, position));

                IsGameFinished();
            }
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

        string data = JsonUtility.ToJson(game2Data);
        File.WriteAllText(dataFolderPath + "/Game_2_" + System.DateTime.Now.ToString("hh-mm-ss") + ".json", data);
    }

    void OnApplicationQuit()
    {
        SaveIntoJson();
    }

    [System.Serializable]
    public class Game2
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

        // generate balloon data constructor
        public Event(string _type, int _position, int _color)
        {

            if (_position > 3)
            {
                _position -= 4;
            }
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = Global.columnPositions[_position];
            color = Global.colors[_color];
        }

        // hit data constructor
        public Event(string _type, string _position)
        {
            timestamp = Time.timeSinceLevelLoad.ToString();
            type = _type;
            position = _position;
        }
    }

}
