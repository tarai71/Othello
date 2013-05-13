using UnityEngine;
using System;
using System.Collections;
using WebSocket4Net;
using LitJson;
using Othello;

class PutBuffer
{
	const int MAX_BUFFER = 64;
	Board.Position[] DataList = new Board.Position[MAX_BUFFER];

	int wptr;
	int rptr;
	
	public PutBuffer()
	{
		Reset();
	}
		
	public void Reset()
	{
		wptr = 0;
		rptr = 0;
	}
		
	public bool Read (out Board.Position pos)
	{
		pos = new Board.Position(0,0);
		if (rptr < wptr) {
			pos = DataList[rptr];
			rptr++;
			return true;
		}
		
		return false;
	}
	
	public bool Write (Board.Position pos)
	{
		if (wptr < MAX_BUFFER) {
			DataList[wptr] = pos;
			wptr++;
			return true;
		}

		Debug.LogError("PutBuffer Overflow!");
		return false;
	}
}

public class connect : MonoBehaviour
{
	WebSocket websocket;
		
	PutBuffer PutList = new PutBuffer();
		
	config compConfig = null;
	menu compMenu = null;
	
	private void websocket_Opened(object sender, EventArgs e)
	{
		websocket.Send("{\"type\":\"entry\", \"name\":\"" + compConfig.MyName + "\"}");
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
		compConfig = GetComponent<config>();
		compMenu = GetComponent<menu>();
	}

	void OnDestroy ()
	{
		DisconnectServer();
	}
	
	public void Send(string message)
	{
	    websocket.Send(message);
	}

	public void putPiece(string data)
	{
		Data d = JsonMapper.ToObject<Data> (data);
		int x, y;
		if(Board.Instance().codeToPos(d.place, out x, out y))
		{
			PutList.Write(new Board.Position(x, y));
		}
	}
	
	public bool ReadPutList(out Board.Position pos)
	{
		return PutList.Read(out pos);
	}
	
	public void ResetPutList()
	{
		PutList.Reset();
	}

	public void ConnectServer()
	{
		Debug.Log("ws://" + compConfig.ServerIP + ":" + compConfig.ServerPort + "/");
		websocket = new WebSocket("ws://" + compConfig.ServerIP + ":" + compConfig.ServerPort + "/");
		websocket.Opened += new EventHandler(websocket_Opened);
		websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs >(websocket_Error);
		websocket.Closed += new EventHandler(websocket_Closed);
		websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
		websocket.Open();
	}

	public void DisconnectServer()
	{
		//if(websocket != null)
		{
			if (compMenu.GetYourID() != "") {
				websocket.Send("{\"type\":\"vsunlock\", \"myid\":\"" + compMenu.GetMyID().ToString() + "\", \"id\":\"" + compMenu.GetYourID().ToString() + "\"}");
			}
			websocket.Send("{\"type\":\"defect\", \"myid\":\"" + compMenu.GetMyID().ToString() + "\"}");
	        websocket.Close();
		}
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