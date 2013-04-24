using UnityEngine;
using System;
using System.Collections;
using WebSocket4Net;
using LitJson;

public class connect : MonoBehaviour
{
	public string SERVER_IP = "172.31.8.144";
	public string PORT = "8888";

	public string ICON_FILE = "";
	
	private WebSocket websocket;
	
	private void websocket_Opened(object sender, EventArgs e)
	{
	    websocket.Send("{\"type\":\"join\",\"user\":\"" + StringTable.PLAYER_NAME + "\"}");
	}
	
	private void websocket_Closed(object sender, EventArgs e)
	{
	}
	
	private void websocket_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
	{
		Debug.Log("サーバに接続できませんでした");
	/*
	  $('#chat-area').empty()
	    .addClass('alert alert-error')
	    .append('<button type="button" class="close" data-dismiss="alert">×</button>',
	      $('<i/>').addClass('icon-warning-sign'),
	      'サーバに接続できませんでした。'
	    );
	 */
	}
	
	private void websocket_MessageReceived(object sender, MessageReceivedEventArgs e)
	{
		Debug.Log(e.Message);
		// 受信したメッセージを復元
//		DataList[ptr++] = JsonMapper.ToObject<Data> (e.Message );
//		if( ptr >= MAX_LINES )
//			ptr = 0;
		
	}

	void Awake ()
	{
		DontDestroyOnLoad (this);
		
		websocket = new WebSocket("ws://" + SERVER_IP + ":" + PORT + "/");
		websocket.Opened += new EventHandler(websocket_Opened);
		websocket.Error += new EventHandler<SuperSocket.ClientEngine.ErrorEventArgs >(websocket_Error);
		websocket.Closed += new EventHandler(websocket_Closed);
		websocket.MessageReceived += new EventHandler<MessageReceivedEventArgs>(websocket_MessageReceived);
		websocket.Open();
	}
	
	void OnDestroy ()
	{
	    websocket.Send("{\"type\":\"defect\",\"user\":\"" + StringTable.PLAYER_NAME + "\"}");
        websocket.Close();
	}
	
}

/*
[System.Serializable]
public class Data {
		public string type = "";
		public string user = "";
		public string text = "";
		public string time = "";
}
*/