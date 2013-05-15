using UnityEngine;
using System.Collections;

public class config : MonoBehaviour
{
	// メニュー用GUISkin定義/
	public GUISkin mySkin;
	
	// コンポーネントキャッシュ/
	menu compMenu = null;
	
	// 初期登録判定フラグ/
	bool IsFirst;
	
	// 名前表示用/
	string myname = "";
	public string MyName
	{
		get { return myname; }
	}
	string save_myname;

	// Othello Matching Server 接続IP/
	string serverip = "54.248.211.43";
	public string ServerIP
	{
		get { return serverip; }
	}
	string save_serverip;

	// Othello Matching Server 接続ポート/
	string serverport = "3000";
	public string ServerPort
	{
		get { return serverport; }
	}
	string save_serverport;

	void Awake ()
	{
		// シーンを遷移してもこのコンポーネントを消さない/
		DontDestroyOnLoad (this);
		
		// コンポーネントキャッシュ/
		compMenu = GetComponent<menu>();

		// PlayerPrefsに設定した"myname"をBase64エンコードして読み込み/
		byte[] nb = System.Convert.FromBase64String((PlayerPrefs.GetString("myname")));
		myname = save_myname = System.Text.Encoding.Unicode.GetString(nb);

		// 初回登録か否かの判定フラグを設定/
		IsFirst = (myname == "");
		if(!IsFirst)
		{
			// PlayerPrefsに設定した"serverip"を読み込み/
			serverip = save_serverip = PlayerPrefs.GetString("serverip");
			// PlayerPrefsに設定した"serverport"を読み込み/
 			serverport = save_serverport = PlayerPrefs.GetString("serverport");
		}
	}

	// Use this for initialization
	void Start ()
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
			if(myname != "" && serverip != "" && serverport != "")
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
		
		if(!IsFirst)
		{
			if(GUILayout.Button(StringTable.CANCEL))
			{
				myname = save_myname;    
				serverip = save_serverip;  
		 		serverport = save_serverport;
				ReturnMenu();
			}
		}
		GUILayout.EndArea();	
	}
	
	void ReturnMenu()
	{
		// メニュー画面への遷移処理/
		this.enabled = false;
		compMenu.enabled = true;
	}
}
