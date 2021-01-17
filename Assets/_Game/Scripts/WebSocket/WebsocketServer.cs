using NativeWebSocket;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Link github Native websocket: https://github.com/endel/NativeWebSocket
public class WebsocketServer : MonoBehaviour {
    public static WebsocketServer Instance;
    //wss://https://chatgolang.herokuapp.com/ws/roomId/userId

    //public static string SERVER_URL = "ws://chatgolang.herokuapp.com/ws/" + ROOM_ID + "/" + USER_ID;
    public static string SERVER_URL = "ws://fishtankserver.herokuapp.com/ws";
    public static string ROOM_ID = "main";
    public static string USER_ID = "user";
    public static float CHAT_HEIGHT = 140f;

    public TMP_InputField chatInput;
    public GameObject chatObj;
    public Transform content;

    WebSocket websocket;

    private void Awake() {
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    async void Start() {
        websocket = new WebSocket(SERVER_URL);

        websocket.OnOpen += () => {
            MyDebug.Log("Connection open!");
        };

        websocket.OnError += (e) => {
            MyDebug.Log("Error! " + e);
        };

        websocket.OnClose += (e) => {
            MyDebug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) => {
            var message = System.Text.Encoding.UTF8.GetString(bytes);

            MyDebug.Log("Receive Message: " + message);

            GameObject go = Instantiate(chatObj, content);
            Vector2 size = content.GetComponent<RectTransform>().sizeDelta;
            content.GetComponent<RectTransform>().sizeDelta = new Vector2(size.x, size.y + CHAT_HEIGHT);
            Vector2 pos = content.GetComponent<RectTransform>().anchoredPosition;
            content.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y + CHAT_HEIGHT);
            go.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>().text = message;
        };

        //InvokeRepeating("SendWebSocketMessage", 0.0f, 0.3f);

        await websocket.Connect();
    }

    void Update() {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    public void SendChat() {
        SendWebSocketMessage();
        chatInput.text = "";
    }

    async void SendWebSocketMessage() {
        if (websocket.State == WebSocketState.Open && chatInput.text != "") {
            //string message = chatInput.text;
            await websocket.SendText(chatInput.text);
        }
    }

    private async void OnApplicationQuit() {
        await websocket.Close();
    }
}