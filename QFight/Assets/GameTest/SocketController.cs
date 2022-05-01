using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

using NativeWebSocket;

class Coord
{
    public string name;
    public float x;
    public float y;
    public float z;
    public Coord(string name, float x, float y, float z)
    {
        this.name = name;
        this.x = x;
        this.y = y;
        this.z = z;
    }
}


class Message<T>
{
    public string type;
    public T data;

    public Message(string type, T data)
    {
        this.type = type;
        this.data = data;
    }
}

public class SocketController : MonoBehaviour
{
    public Terrain terrain;
    private WebSocket websocket;
    public NativeQ nativeQ;

    private float timer = 0f;

    // Start is called before the first frame update
    async void Start()
    {
        websocket = new WebSocket("ws://localhost:8080/name");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error! " + e);
        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            Debug.Log("OnMessage!");
            Debug.Log(bytes);

            // getting the message as a string
            var strMessage = System.Text.Encoding.UTF8.GetString(bytes);
            Debug.Log("OnMessage! " + strMessage);

            Message<object> unkownMessage = JsonConvert.DeserializeObject<Message<object>>(strMessage);

            if (unkownMessage.type.Equals("run-script"))
            {
                Message<string> scriptMessage = JsonConvert.DeserializeObject<Message<string>>(strMessage);
                string code = scriptMessage.data;
                Debug.Log("Run script " + code);
                nativeQ.ExecuteCode(code);
            }
        };

        // waiting for messages
        await websocket.Connect();
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
        timer += Time.deltaTime;

        if (timer > 3f)
        {
            UpdateCoords();
            timer = 0f;
        }
#endif
    }

    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    async void UpdateCoords()
    {
        Debug.Log("Invoke UpdateCoords");
        if (websocket.State == WebSocketState.Open)
        {
            GameObject[] units = GameObject.FindGameObjectsWithTag("Unit");
            List<Coord> coords = new List<Coord>();

            foreach (GameObject u in units)
            {
                Vector3 relativeCoords = terrain.transform.InverseTransformPoint(u.transform.position);
                coords.Add(new Coord(u.name, relativeCoords.x, relativeCoords.y, relativeCoords.z));
            }

            Message<Coord[]> message = new Message<Coord[]>("update-coords", coords.ToArray());

            Debug.Log("send coord " + JsonConvert.SerializeObject(message));

            await websocket.SendText(JsonConvert.SerializeObject(message));
        }
    }
}
