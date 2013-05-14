using UnityEngine;
using System.Collections;

public class config : MonoBehaviour
{
	public GUISkin mySkin;
	
	menu compMenu = null;
	connect compConnect = null;
	
	string myname = "";
	public string MyName
	{
		get { return myname; }
	}

	string serverip = "54.248.211.43";
	public string ServerIP
	{
		get { return serverip; }
	}

	string serverport = "3000";
	public string ServerPort
	{
		get { return serverport; }
	}

	void Awake ()
	{
		DontDestroyOnLoad (this);
		compMenu = GetComponent<menu>();
		compConnect = GetComponent<connect>();

		byte[] nb = System.Convert.FromBase64String((PlayerPrefs.GetString("myname")));
		myname = System.Text.Encoding.Unicode.GetString(nb);
	}

	// Use this for initialization
	void Start ()
	{
	}
	
	void Update ()
	{
	}
	
	void OnGUI ()
	{
		GUI.skin = mySkin;
		
		GUILayout.BeginArea(new Rect (10, 10, Screen.width-20, Screen.height-20));
		GUILayout.Space(50);

		GUILayout.BeginHorizontal();
		GUILayout.Label(StringTable.NAME + ":", GUILayout.MinWidth(200));
		myname = GUILayout.TextField(myname);
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label(StringTable.SERVERIP + ":", GUILayout.MinWidth(200));
		serverip = GUILayout.TextField(serverip);
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
		
		GUILayout.BeginHorizontal();
		GUILayout.Label(StringTable.SERVERPORT + ":", GUILayout.MinWidth(200));
		serverport = GUILayout.TextField(serverport);
		GUILayout.EndHorizontal();
		GUILayout.Space(10);
		
		if(GUILayout.Button(StringTable.REGIST))
		{
			Check();
		}
		
		if(GUILayout.Button(StringTable.CANCEL))
		{
			ReturnMenu();
		}
		GUILayout.EndArea();	
	}
	
	void Check()
	{
		if(myname != "")
		{
			Debug.Log(StringTable.NAME + ":" + myname);
			Debug.Log(StringTable.SERVERIP + ":" + serverip);
			Debug.Log(StringTable.SERVERPORT + ":" + serverport);
			
			byte[] nb = System.Text.Encoding.Unicode.GetBytes (myname);
			
			PlayerPrefs.SetString("myname", System.Convert.ToBase64String(nb));
			PlayerPrefs.SetString("serverip", serverip);
			PlayerPrefs.SetString("serverport", serverport);
			PlayerPrefs.Save();
		
			ReturnMenu();
		}
	}
	
	void ReturnMenu()
	{
		this.enabled = false;
		compMenu.enabled = true;
		compConnect.ConnectServer();
	}
}
