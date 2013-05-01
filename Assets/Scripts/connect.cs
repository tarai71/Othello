using UnityEngine;
using System;
using System.Collections;
using WebSocket4Net;
using LitJson;

public class connect : MonoBehaviour
{
	public string SERVER_IP = "172.31.8.144";
	public string PORT = "3000";
	
	private static WebSocket websocket;
		
	const int MAX_BUFFER = 60;
	private Data[] DataList = new Data[MAX_BUFFER];
	private int wptr = 0;
	private int rptr = 0;

	
	private void websocket_Opened(object sender, EventArgs e)
	{
//		websocket.Send("{\"type\":\"entry\",\"parameter\":{\"name\":\"arai\",\"time\":\"\"}}");
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
		Data data = JsonMapper.ToObject<Data> (e.Message);
		if (data.type == "join") {
			Debug.Log("[websocket_MessageReceived] \"Join\" type recieved");
		} else if( data.type == "entrylist") {
			Debug.Log("[websocket_MessageReceived] \"Entey\" type recieved");
		} else if( data.type == "put") {
			DataList[wptr++] = data;
			if (wptr >= MAX_BUFFER)
				wptr = 0;
			Debug.Log("[websocket_MessageReceived] \"Put\" type recieved");
		} else {
			Debug.Log("[websocket_MessageReceived] Undeined type recieved");
		}
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
		for (int i=0; i<MAX_BUFFER; i++, rptr++) {
			if (rptr >= MAX_BUFFER)
				rptr = 0;

			if (DataList[rptr] != null) {
				GameObject.FindWithTag("GameController").SendMessage("putPiece", main.codeToPos(DataList[rptr].place));
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
		public string time = "";
		public string name = "";
		public string place = "";
}
