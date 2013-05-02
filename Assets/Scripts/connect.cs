using UnityEngine;
using System;
using System.Collections;
using WebSocket4Net;
using LitJson;

public class connect : MonoBehaviour
{
	public string SERVER_IP = "172.31.8.144";
	public string PORT = "3000";
	
	WebSocket websocket;
		
	const int MAX_BUFFER = 60;
	Data[] DataList = new Data[MAX_BUFFER];
	int wptr = 0;
	int rptr = 0;
	
	menu compMenu = null;

	
	private void websocket_Opened(object sender, EventArgs e)
	{
		websocket.Send("{\"type\":\"entry\", \"name\":\"" + compMenu.configData.name + "\"}");
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
			Debug.Log("[websocket_MessageReceived] \"Entry\" type recieved");
			compMenu.SetEntry(data.list);
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
		
		compMenu = GetComponent<menu>();
		
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
				GameObject.FindWithTag("GameController").SendMessage("putPiece", GameObject.FindWithTag("GameController").GetComponent<main>().codeToPos(DataList[rptr].place));
			}
		}
	}
	
	void OnDestroy ()
	{
	    websocket.Send("{\"type\":\"defect\",\"name\":\"" + StringTable.PLAYER_NAME + "\"}");
        websocket.Close();
	}
	
	public void Send(string message)
	{
	    websocket.Send(message);
	}
}

[System.Serializable]
public class Data {
		public string type = "";
		public string name = "";
		public string time = "";
		public string place = "";
		public int id = -1;
		public Entry[] list;
}
[System.Serializable]
public class Entry {
	public string type = "";
	public string name = "";
	public string time = "";
	public bool own = false;
	public int id = -1;
}