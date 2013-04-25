using UnityEngine;
using System;
using System.Collections;
using WebSocket4Net;
using LitJson;

public class connect : MonoBehaviour
{
	public string SERVER_IP = "172.31.8.144";
	public string PORT = "3000";

	public string ICON_FILE = "";
	
	private static WebSocket websocket;
	
	const int MAX_LINES = 10;
	private Data[] DataList = new Data[MAX_LINES];
	private int ptr = 0;

	
	private void websocket_Opened(object sender, EventArgs e)
	{
	    websocket.Send("{\"type\":\"join\",\"user\":\"" + StringTable.PLAYER_NAME + "\"}");
	}
	
	private void websocket_Closed(object sender, EventArgs e)
	{
	}
	
	private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
	{
		Debug.Log(StringTable.CANT_CONNECT);
	}
	
	private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
	{
		Debug.Log(e.Message);
		DataList[ptr++] = JsonMapper.ToObject<Data> (e.Message );
		if( ptr >= MAX_LINES )
			ptr = 0;
	}

	void Awake ()
	{
		DontDestroyOnLoad (this);
		
		Debug.Log("ws://" + SERVER_IP + ":" + PORT + "/");
		websocket = new WebSocket("ws://" + SERVER_IP + ":" + PORT + "/");
		websocket.Opened += new EventHandler(websocket_Opened);
		websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs >(websocket_Error);
		websocket.Closed += new EventHandler(websocket_Closed);
		websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
		websocket.Open();
	}
	
	void Update ()
	{
		for (int i=0, p=ptr; i<MAX_LINES; i++) {
			p--;
			if (p < 0)
				p = MAX_LINES-1;

			if (DataList[p] != null) {
				if (DataList[p].type == "put") {
					GameObject.FindWithTag("GameController").SendMessage("putPiece", main.codeToPos(DataList[p].place));
				}
			}
		}
	}
	
	void OnDestroy ()
	{
	    websocket.Send("{\"type\":\"defect\",\"user\":\"" + StringTable.PLAYER_NAME + "\"}");
        websocket.Close();
	}
	
	public static void Send(string message)
	{
	    websocket.Send(message);
	}
}

[System.Serializable]
public class Data {
		public string type = "";
		public string user = "";
		public string text = "";
		public string place = "";
		public string time = "";
}
