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
	private DataPut[] PutList = new DataPut[MAX_BUFFER];
	private int wptr = 0;
	private int rptr = 0;

	
	private void websocket_Opened(object sender, EventArgs e)
	{
//	    websocket.Send("{\"type\":\"entry\",\"message\":{\"name\":\"" + StringTable.PLAYER_NAME + "\"}}");
		//JsonData jd = "{'type':'entry','message':'{\'name\':\'arai\'}'";

		websocket.Send("{\"type\":\"entry\",\"message\":{\"name\":\"arai\",\"time\":\"\"}}");
		//websocket.Send(jd.ToJson());
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
			DataJoin join = JsonMapper.ToObject<DataJoin> (data.message);
			Debug.Log("[websocket_MessageReceived] \"Join\" type recieved");
		} else if( data.type == "entry") {
			DataEntry entry = JsonMapper.ToObject<DataEntry> (data.message);
			Debug.Log("[websocket_MessageReceived] \"Entey\" type recieved");
		} else if( data.type == "put") {
			PutList[wptr++] = JsonMapper.ToObject<DataPut> (data.message);
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

			if (PutList[rptr] != null) {
				GameObject.FindWithTag("GameController").SendMessage("putPiece", main.codeToPos(PutList[rptr].place));
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
		public string message = "";
}
public class DataJoin {
		public string time = "";
}
public class DataEntry {
		public string name = "";
}
public class DataPut {
		public string place = "";
		public string time = "";
}
