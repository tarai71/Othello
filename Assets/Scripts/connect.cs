using UnityEngine;
using System;
using System.Collections;
using WebSocket4Net;
using LitJson;

public class connect : MonoBehaviour
{
	public string SERVER_IP = "54.248.211.43";
	public string PORT = "3000";
	
	WebSocket websocket;
		
	const int MAX_BUFFER = 64;
	Vector2[] DataList = new Vector2[MAX_BUFFER];
	public int wptr = 0;
	public int rptr = 0;
	
	menu compMenu = null;
	public main compMain = null;
	
	private void websocket_Opened(object sender, EventArgs e)
	{
		websocket.Send("{\"type\":\"entry\", \"name\":\"" + ConfigData.name + "\"}");
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
			compMenu.SetEntry(data.list, false);
		} else if( data.type == "updatelock") {
			Debug.Log("[websocket_MessageReceived] \"UpdateLock\" type recieved");
			compMenu.SetEntry(data.list, true);
		} else if( data.type == "put") {
			putPiece(e.Message);
			Debug.Log("[websocket_MessageReceived] \"Put\" type recieved");
		} else if( data.type == "vslock") {
			compMenu.SetLocked(data.myid);
		} else if( data.type == "vsunlock") {
			compMenu.SetUnlock();
		} else if( data.type == "start") {
			compMenu.StartGame(data.option);
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

	void OnDestroy ()
	{
		if (compMenu.getYourID() != "") {
			websocket.Send("{\"type\":\"vsunlock\", \"myid\":\"" + compMenu.getMyID().ToString() + "\", \"id\":\"" + compMenu.getYourID().ToString() + "\"}");
		}
		websocket.Send("{\"type\":\"defect\", \"myid\":\"" + compMenu.getMyID().ToString() + "\"}");
        websocket.Close();
	}
	
	public bool ReadPutBuffer (out Vector2 pos)
	{
		pos = Vector2.zero;
		if (rptr < wptr) {
			pos = DataList[rptr];
			rptr++;
			return true;
		}
		
		return false;
	}
	
	bool WritePutBuffer (Vector2 pos)
	{
		if (wptr < MAX_BUFFER) {
			DataList[wptr] = pos;
			wptr++;
			return true;
		}
		return false;
	}
	
	public void Send(string message)
	{
	    websocket.Send(message);
	}

	public void putPiece(string data)
	{
		Data d = JsonMapper.ToObject<Data> (data);
		WritePutBuffer(compMenu.codeToPos(d.place));
	}

}

[System.Serializable]
public class Data {
		public string type = "";
		public string name = "";
		public string time = "";
		public string place = "";
		public string myid = "";
		public string id = "";
		public int[] option = {0,0,0,0,0}; 
		public bool locked = false;
		public Entry[] list;
}
[System.Serializable]
public class Entry {
	public string type = "";
	public string name = "";
	public string time = "";
	public bool own = false;
	public bool locked = false;
	public string id = "";
}