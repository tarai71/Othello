using UnityEngine;
using System.Collections;
using LitJson;

public class menu : MonoBehaviour
{
	public GUISkin mySkin;

	public enum LOCK_TYPE {
		FREE = 0,
		LOCK,
		LOCKED
	}
	
	Rect[] windowRect = {
		new Rect ( 10, 140, (Screen.width-20)/2-5, 20),
		new Rect ( 20+(Screen.width-20)/2-5, 140, (Screen.width-20)/2-5, 20),//(10, 160, 200, 20),
		new Rect (220,  60, 200, 20),
		new Rect (220, 160, 200, 20),
		new Rect ( 10, 420, 700, 200)//(440, 60, 300, 20)
	};
	int[] option = {0,0,0,0,0};
	Vector2 scrollPosition = Vector2.zero;
	float[] timeTable = {
		0f, 5f, 10f, 15f, 20f, 25f, 30f
	};
		
	config compConfig = null;
	connect compConnect = null;

	Hashtable entryList = new Hashtable();
	string[] entryIdList = {};
	string[] entryNameList = {};
	
	LOCK_TYPE lockType = LOCK_TYPE.FREE;
	bool IsGameStart = false;
	bool IsGameEnd = false;
	string myID = "";
	string yourID = "";
	
	void Update ()
	{
		if (IsGameStart)
		{
			IsGameStart = false;
			this.enabled = false;
			compConnect.ResetPutList();
			Application.LoadLevel("Main");
		}
	}
	
	void OnGUI ()
	{
		GUI.skin = mySkin;

			windowRect[0] = GUILayout.Window (0, new Rect ( 10, 140, (Screen.width-20)/2-5, 20), MakeSelectWindow, StringTable.SENTE);
			windowRect[1] = GUILayout.Window (1, new Rect ( 20+(Screen.width-20)/2-5, 140, (Screen.width-20)/2-5, 20), MakeSelectWindow, StringTable.GOTE);
//			windowRect[2] = GUILayout.Window (2, windowRect[2], MakeGuideWindow, StringTable.GUIDE);
//			windowRect[3] = GUILayout.Window (3, windowRect[3], MakeTimeWindow, StringTable.TIME);
			windowRect[4] = GUILayout.Window (4, new Rect ( 10, 420, Screen.width-20, Screen.height-(420+20)), MakeEntryWindow, StringTable.ENTRY);

		GUILayout.BeginArea(new Rect (10, 10, Screen.width-20, Screen.height-20));
		GUILayout.Space(10);
		if(lockType == LOCK_TYPE.LOCKED)
		{
			GUILayout.Label(GetYourName() + StringTable.LOCKED);
		} else {
			
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(StringTable.START)) {
				StartGame(option);
				if(lockType != LOCK_TYPE.FREE)
				{
					compConnect.Send("{\"type\":\"startgame\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + ((Entry)entryList[yourID]).id.ToString() + "\", \"option\":" + JsonMapper.ToJson(option) +  "}");
				}
			}

			if(GUILayout.Button(StringTable.INITIALIZE, GUILayout.MaxWidth(200))) {
				compConnect.DisconnectServer();
				this.enabled = false;
				compConfig.enabled= true;
			}

			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();	
	}
	
	void Awake ()
	{
		DontDestroyOnLoad (this);
		
		compConfig = GetComponent<config>();
		compConnect = GetComponent<connect>();
	}

	// Use this for initialization
	void Start () {
		if(compConfig.MyName == "")
		{
			this.enabled = false;
			compConfig.enabled= true;
			compConnect.DisconnectServer();
		}
		else
		{
			compConnect.ConnectServer();
		}
	}
	
	void MakeSelectWindow (int id)
	{
		GUILayout.Space (10);
		option[id] = GUILayout.SelectionGrid(option[id], new string[]{StringTable.HUMAN,StringTable.COMPUTER}, 1);
		GUILayout.FlexibleSpace ();
	}

	void MakeGuideWindow (int id)
	{
		GUILayout.Space (10);
		option[id] = GUILayout.SelectionGrid(option[id], new string[]{StringTable.ON,StringTable.OFF}, 1);
		GUILayout.FlexibleSpace ();
	}

	void MakeTimeWindow (int id)
	{
		GUILayout.Space (10);
		option[id] = GUILayout.SelectionGrid(option[id], new string[]{StringTable.NO_LIMIT,StringTable.SEC5,StringTable.SEC10,StringTable.SEC15,StringTable.SEC20,StringTable.SEC25,StringTable.SEC30}, 1);
		GUILayout.FlexibleSpace ();
	}

	void MakeEntryWindow (int id)
	{
		int old = option[id];

		GUILayout.Space (10);

		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.Width (Screen.width-40), GUILayout.Height (Screen.height-500));

		option[id] = GUILayout.SelectionGrid(option[id], entryNameList, 1);
		GUILayout.FlexibleSpace ();
		
		if (old != option[id]) {
			if (((Entry)entryList[entryIdList[option[id]]]).locked && (!((Entry)entryList[entryIdList[option[id]]]).own || (lockType != LOCK_TYPE.LOCK))) {
				option[id] = old;
			} else {
				if (!((Entry)entryList[entryIdList[old]]).own) {
					compConnect.Send("{\"type\":\"unlock\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + ((Entry)entryList[entryIdList[old]]).id.ToString() + "\"}");
					SetUnlock();
				}
				if (!((Entry)entryList[entryIdList[option[id]]]).own) {
					compConnect.Send("{\"type\":\"lock\", \"myid\":\"" + myID.ToString() + "\", \"id\":\"" + ((Entry)entryList[entryIdList[option[id]]]).id.ToString() + "\"}");
					SetLock();
				}
			}
		}
		
		GUILayout.EndScrollView();
	}

	public void StartGame (int[] opt) {
		IsGameStart = true;
		option = opt;
	}

	public void EndGame () {
		IsGameEnd = true;
	}

	public bool GetEndGame () {
		return IsGameEnd;
	}

	public void SetUnlock()
	{
		lockType = LOCK_TYPE.FREE;
		yourID = "";
	}
	public void SetLock()
	{
		lockType = LOCK_TYPE.LOCK;
		yourID = ((Entry)entryList[entryIdList[option[4]]]).id;
	}
	public void SetLocked(string id)
	{
		lockType = LOCK_TYPE.LOCKED;
		yourID = id;
	}
	
	public void SetEntry(Entry[] list, bool isLock)
	{
		entryList.Clear() ;
		entryNameList = new string[list.Length];
		entryIdList = new string[list.Length];
		for(int i=0; i<list.Length; i++) {
			entryList[list[i].id] = list[i];
			entryIdList[i] = list[i].id;
			if (list[i].own) {
				entryNameList[i] = StringTable.NO_VS + "[" + list[i].id + "]";
				if(!isLock) {
					option[4] = i;
				}
				myID = list[i].id;
			} else {
				entryNameList[i] = list[i].name + "[" + list[i].id + "]";
			}
			if (list[i].locked && (!list[i].own || (lockType != LOCK_TYPE.LOCK))) {
				entryNameList[i] += "*";
			}
		}
	}

	public bool GetGuideEnable () 
	{
		return (option[2] == 0);
	}

	public float GetLimitTime () 
	{
		return timeTable[option[3]];
	}

	public string GetYourID ()
	{
		return yourID;
	}
	public string GetMyID ()
	{
		return myID;
	}
	
	public string GetYourName ()
	{
		return (yourID == "")? "" : ((Entry)entryList[yourID]).name;
	}

	public string GetMyName ()
	{
		return (myID == "")? "" : ((Entry)entryList[myID]).name;
	}
	
	public LOCK_TYPE getLockType () 
	{
		return lockType;
	}

	public int GetKind (int side)
	{
		if (side != 0 && side != 1) {
			return 0;
		}
		
		return option[side];
	}

}
