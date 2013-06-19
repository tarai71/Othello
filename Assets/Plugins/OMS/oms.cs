using System;
using System.Collections;
using System.Threading;
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
	
			Console.WriteLine("PutBuffer Overflow!");
			return false;
		}
	}

	public class connect
	{
		public delegate void CallbackUpdateEntry(Entry[] list, bool isLock);
		public delegate void CallbackLock(string id);
		public delegate void CallbackUnlock();
		public delegate void CallbackStartGame(int[] opt);
		public delegate void CallbackEndGame();
		public delegate void CallbackPutPiece(string location);
		public delegate void CallbackTimeOut();

		public CallbackUpdateEntry OnUpdateEntry = null;
		public CallbackLock OnLock = null;
		public CallbackUnlock OnUnlock = null;
		public CallbackStartGame OnStartGame = null;
		public CallbackEndGame OnEndGame = null;
		public CallbackPutPiece OnPutPiece = null;
		public CallbackTimeOut OnTimeOut = null;

		WebSocket websocket = null;
		string myname = "";
		Timer tm = null;
		bool keepalive = true;
			
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
			Console.WriteLine("Can't connet");
		}
		
		private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
		{
			Data data = JsonMapper.ToObject<Data> (e.Message);
			if (data.type == "join") {
			} else if( data.type == "entrylist") {
				if(OnUpdateEntry != null) {
					OnUpdateEntry(data.list, false);
				}
			} else if( data.type == "updatelock") {
				if(OnUpdateEntry != null) {
					OnUpdateEntry(data.list, true);
				}
			} else if( data.type == "put") {
				WritePutList(data.place);
			} else if( data.type == "lock") {
				if(OnLock != null) {
					OnLock(data.myid);
				}
			} else if( data.type == "unlock") {
				if(OnUnlock != null) {
					OnUnlock();
				}
			} else if( data.type == "startgame") {
				if(OnStartGame != null) {
					OnStartGame(data.option);
				}
			} else if( data.type == "endgame") {
				if(OnEndGame != null) {
					OnEndGame();
				}
			} else if( data.type == "keepalive") {
				keepalive = true;
				websocket.Send("{\"type\":\"keepalive\", \"myid\":\"" + data.myid + "\", \"name\":\"" + myname + "\"}");
			} else {
				Console.WriteLine("[websocket_MessageReceived] Undeined type recieved");
			}
		}
	
		
		public void ConnectServer(string url, string name)
		{
#if NETWORK_ENABLE
			if(websocket == null)
			{
				myname = name;
				Console.WriteLine(url);
				websocket = new WebSocket(url);
				websocket.Opened += new EventHandler(websocket_Opened);
				websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs >(websocket_Error);
				websocket.Closed += new EventHandler(websocket_Closed);
				websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
				websocket.Open();

				// 一定間隔ごとに呼び出すメソッドをTimerCallbackとして登録.
				TimerCallback timerCallback = new TimerCallback(ThreadMethod);
				
				// 5秒待ってから、2秒ごとにtimerCallbackメソッドを呼び出す.
				tm = new Timer(timerCallback, this, 5 * 1000, 6 * 1000);
			}
			else
			{
				Console.WriteLine("It is already connected!");
			}	
#endif
		}

		// 2秒ごとに実行されるメソッド.
		private static void ThreadMethod(object state)
		{
				connect c = (connect)state;
				if(c.keepalive)
				{
					c.keepalive = false;
					return;
				}
		
				c.OnTimeOut();
		}
		
		public void DisconnectServer(string myID)
		{
#if NETWORK_ENABLE
			if(websocket != null && websocket.State != WebSocketState.Closed && websocket.State != WebSocketState.Closing)
			{
				websocket.Send("{\"type\":\"defect\", \"myid\":\"" + myID.ToString() + "\"}");
		        websocket.Close();
			}
			websocket = null;
			myname = "";
			tm.Dispose();
			tm = null;
#endif
		}
	
		public void Lock(string myID, string yourID)
		{
#if NETWORK_ENABLE
			if(websocket != null)
			{
				websocket.Send("{\"type\":\"lock\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\"}");
			}
#endif
		}		
	
		public void Unlock(string myID, string yourID)
		{
#if NETWORK_ENABLE
			if(websocket != null)
			{
				websocket.Send("{\"type\":\"unlock\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\"}");
			}
#endif
		}		
	
		public void StartGame(string myID, string yourID, int[] option)
		{
#if NETWORK_ENABLE
			if(websocket != null)
			{
				websocket.Send("{\"type\":\"startgame\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\", \"option\":" + JsonMapper.ToJson(option) +  "}");
			}
#endif
		}		
	
		public void EndGame(string myID, string yourID)
		{
#if NETWORK_ENABLE
			if(websocket != null)
			{
				websocket.Send("{\"type\":\"endgame\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + yourID.ToString() + "\"}");
			}
#endif
		}		
	
		public void PutPiece(string yourID, string location)
		{
#if NETWORK_ENABLE
			if(websocket != null)
			{
				websocket.Send("{\"type\":\"put\",\"id\":\"" + yourID.ToString() + "\",\"place\":\"" + location + "\"}");
			}
#endif
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
		public bool keepalive = false;
	}
	
}