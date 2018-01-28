using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    private InputField field;
    public GameObject input;

    public GameObject OwnTextPrefab;
    public GameObject PartnerTextPrefab;
    private float textOffset;

    public GameObject view;

    public static Messenger Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    private void Start()
    {
        field = input.GetComponent<InputField>();
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetButtonDown("Submit") && GameServer.Instance.gameStatus == GameStatus.Ongoing) send();
    }

    public void focusInputField()
    {
        field.Select();
        field.ActivateInputField();
    }

    public void receiveMsg(string msg)
    {
        gameObject.SetActive(true);
        Debug.Log(msg);
        displayMsg(msg, PartnerTextPrefab);
        focusInputField();
    }

    private void send()
    {
        LocalPlayerNetworkConnection connectionObj =
            FindObjectsOfType<LocalPlayerNetworkConnection>().First(l => l.isLocalPlayer);
        if (field != null && view != null)
        {
            displayMsg(field.text, OwnTextPrefab);
            if (connectionObj != null)
            {
                Debug.Log("Sending Message via network");
                connectionObj.SendChatMessage(field.text);
            }
            else
            {
                Debug.Log("Message was not sent");
            }

            field.text = "";
            focusInputField();
        }
    }

    private void displayMsg(string msg, GameObject prefab)
    {
        GameObject chatText = Instantiate(prefab, Vector3.zero, Quaternion.identity);
        chatText.GetComponentInChildren<Text>().text = msg;
        chatText.transform.SetParent(view.transform, false);
    }
}