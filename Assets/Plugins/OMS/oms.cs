using System;
using System.Collections;
using WebSocket4Net;
using LitJson;

namespace OMS
{
	public class Position {
		public int x;
		public int y;
		
		public Position(int ix, int iy)
		{
			x = ix;
			y = iy;
		}
	}
		
	class PutBuffer
	{
		const int MAX_BUFFER = 64;
		string[] DataList = new string[MAX_BUFFER];
	
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
			
		public bool Read (out string localtion)
		{
			localtion = "";
			if (rptr < wptr) {
				localtion = DataList[rptr];
				rptr++;
				return true;
			}
			
			return false;
		}
		
		public bool Write (string localtion)
		{
			if (wptr < MAX_BUFFER) {
				DataList[wptr] = localtion;
				wptr++;
				return true;
			}
	
	//		Debug.LogError("PutBuffer Overflow!");
			return false;
		}
	}

	public class connect
	{
		public delegate void CallbackUpdateEntry(Entry[] list);
		public delegate void CallbackLock(string id);
		public delegate void CallbackUnlock();
		public delegate void CallbackStartGame(int[] opt);
		public delegate void CallbackEndGame();
		public delegate void CallbackPutPiece(string location);

		public CallbackUpdateEntry OnUpdateEntry = null;
		public CallbackLock OnLock = null;
		public CallbackUnlock OnUnlock = null;
		public CallbackStartGame OnStartGame = null;
		public CallbackEndGame OnEndGame = null;
		public CallbackPutPiece OnPutPiece = null;

		WebSocket websocket = null;
		string myname = "";
			
		PutBuffer PutList = new PutBuffer();
			
		private void websocket_Opened(object sender, EventArgs e)
		{
			websocket.Send("{\"type\":\"entry\", \"name\":\"" + myname + "\"}");
		}
		
		private void websocket_Closed(object sender, EventArgs e)
		{
		}
		
		private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
		{
	//		Debug.Log(StringTable.CANT_CONNECT);
		}
		
		private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			Data data = JsonMapper.ToObject<Data> (e.Message);
			if (data.type == "join") {
			} else if( data.type == "entrylist") {
				if(OnUpdateEntry != null) {
					OnUpdateEntry(data.list);
				}
			} else if( data.type == "updatelock") {
				if(OnUpdateEntry != null) {
					OnUpdateEntry(data.list);
				}
			} else if( data.type == "put") {
				if(OnUpdateEntry != null) {
					WritePutList(data.place);
				}
			} else if( data.type == "lock") {
				if(OnUpdateEntry != null) {
					OnLock(data.myid);
				}
			} else if( data.type == "unlock") {
				if(OnUpdateEntry != null) {
					OnUnlock();
				}
			} else if( data.type == "startgame") {
				if(OnUpdateEntry != null) {
					OnStartGame(data.option);
				}
			} else if( data.type == "endgame") {
				if(OnUpdateEntry != null) {
					OnEndGame();
				}
			} else {
				//Debug.Log("[websocket_MessageReceived] Undeined type recieved");
			}
		}
	
		
		public void ConnectServer(string url, string name)
		{
			if(websocket == null)
			{
				myname = name;
				//Debug.Log("ws://" + compConfig.ServerIP + ":" + compConfig.ServerPort + "/");
				websocket = new WebSocket(url);
				websocket.Opened += new EventHandler(websocket_Opened);
				websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs >(websocket_Error);
				websocket.Closed += new EventHandler(websocket_Closed);
				websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
				websocket.Open();
			}
			else
			{
				//Debug.Log("It is already connected!");
			}	
		}
	
		public void DisconnectServer(string myID)
		{
			if(websocket.State != WebSocketState.Closed && websocket.State != WebSocketState.Closing)
			{
				websocket.Send("{\"type\":\"defect\", \"myid\":\"" + myID.ToString() + "\"}");
		        websocket.Close();
				websocket = null;
				myname = "";
			}
		}
	
		public void Lock(string myID, string yourID)
		{
			websocket.Send("{\"type\":\"lock\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\"}");
		}		
	
		public void Unlock(string myID, string yourID)
		{
			websocket.Send("{\"type\":\"unlock\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\"}");
		}		
	
		public void StartGame(string myID, string yourID, int[] option)
		{
			websocket.Send("{\"type\":\"startgame\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\", \"option\":" + JsonMapper.ToJson(option) +  "}");
		}		
	
		public void EndGame(string myID, string yourID)
		{
			websocket.Send("{\"type\":\"endgame\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\"}");
		}		
	
		public void PutPiece(string yourID, string location)
		{
			websocket.Send("{\"type\":\"put\",\"id\":\"" + yourID.ToString() + "\",\"place\":\"" + location + "\"}");
		}		

		public void WritePutList(string location)
		{
			PutList.Write(location);
		}		

		public bool ReadPutList(out string location)
		{
			return PutList.Read(out location);
		}
		
		public void ResetPutList()
		{
			PutList.Reset();
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
	
}