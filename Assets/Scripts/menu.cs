using UnityEngine;
using System;
using System.Collections;
using LitJson;
using OMS;

public class menu : MonoBehaviour
{
	public GUISkin mySkin;

	public enum LOCK_TYPE {
		FREE = 0,
		LOCK,
		LOCKED
	}
	
	public OMS.connect oms;

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
			IsGameEnd = false;
			this.enabled = false;
			oms.ResetPutList();
			Application.LoadLevel("Main");
		}
	}
	
	void OnGUI ()
	{
		GUI.skin = mySkin;

		GUILayout.BeginArea(new Rect (10, 10, Screen.width-20, Screen.height-20));
		GUILayout.Space(10);
		if(lockType == LOCK_TYPE.LOCKED)
		{
			GUILayout.Label(GetYourName() + StringTable.LOCKED);
		} else {
#if AI_ENABLE
			windowRect[0] = GUILayout.Window (0, new Rect ( 10, 140, (Screen.width-20)/2-5, 20), MakeSelectWindow, StringTable.SENTE);
			windowRect[1] = GUILayout.Window (1, new Rect ( 20+(Screen.width-20)/2-5, 140, (Screen.width-20)/2-5, 20), MakeSelectWindow, StringTable.GOTE);
#endif
//			windowRect[2] = GUILayout.Window (2, windowRect[2], MakeGuideWindow, StringTable.GUIDE);
//			windowRect[3] = GUILayout.Window (3, windowRect[3], MakeTimeWindow, StringTable.TIME);
#if NETWORK_ENABLE
			windowRect[4] = GUILayout.Window (4, new Rect ( 10, 420, Screen.width-20, Screen.height-(420+20)), MakeEntryWindow, StringTable.ENTRY);
#endif
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(StringTable.START)) {
				StartGame(option);
				if(lockType != LOCK_TYPE.FREE)
				{
					oms.StartGame(myID, ((Entry)entryList[yourID]).id, option);
				}
				else
				{
					oms.DisconnectServer(myID);
				}
			}

#if NETWORK_ENABLE
			if(GUILayout.Button(StringTable.INITIALIZE, GUILayout.MaxWidth(200))) {
				oms.DisconnectServer(myID);
				this.enabled = false;
				compConfig.enabled= true;
			}
#endif
			
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();	
	}
	
	void Awake ()
	{
		DontDestroyOnLoad (this);
		
		compConfig = GetComponent<config>();
		
		oms = new OMS.connect();
		oms.OnLock = othello_OnLock;
		oms.OnUnlock = othello_OnUnlock;
		oms.OnStartGame = othello_OnStartGame;
		oms.OnEndGame = othello_OnEndGame;
		oms.OnPutPiece = othello_OnPutPiece;
		oms.OnUpdateEntry = othello_OnUpdateEntry;
		oms.OnTimeOut = othello_OnTimeOut;
	}
	
	private void othello_OnLock(string myid)
	{
		SetLocked(myid);
	}
	private void othello_OnUnlock()
	{
		SetUnlock();
	}
	private void othello_OnStartGame(int[] option)
	{
		StartGame(option);
	}
	private void othello_OnEndGame()
	{
		EndGame();
	}
	private void othello_OnPutPiece(string location)
	{
	}
	private void othello_OnUpdateEntry(OMS.Entry[] list, bool isLock)
	{
		UpdateEntryList(list, isLock);
	}
	private void othello_OnTimeOut()
	{
		Debug.Log("TimeOut");
		Debug.Log("Try Re-connenct!");
		oms.ConnectServer("ws://" + compConfig.ServerIP + ":" + compConfig.ServerPort + "/", compConfig.MyName);
	}
	

	// Use this for initialization
	void Start () {
#if NETWORK_ENABLE
		if(compConfig.MyName == "")
		{
			this.enabled = false;
			compConfig.enabled= true;
			oms.DisconnectServer(myID);
		}
		else
		{
			oms.ConnectServer("ws://" + compConfig.ServerIP + ":" + compConfig.ServerPort + "/", compConfig.MyName);
		}
#endif
	}

	void OnDestroy ()
	{
		if(lockType != LOCK_TYPE.FREE)
		{
			oms.Unlock(myID, yourID);
		}
		oms.DisconnectServer(myID);
	}

	void OnEnable () {
		if(compConfig.MyName != "") {
			oms.ConnectServer("ws://" + compConfig.ServerIP + ":" + compConfig.ServerPort + "/", compConfig.MyName);
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
					oms.Unlock(myID, ((Entry)entryList[entryIdList[old]]).id);
					SetUnlock();
				}
				if (!((Entry)entryList[entryIdList[option[id]]]).own) {
					oms.Lock(myID, ((Entry)entryList[entryIdList[option[id]]]).id.ToString());
					SetLock();
				}
			}
		}
		
		GUILayout.EndScrollView();
	}

	void StartGame (int[] opt) {
		IsGameStart = true;
		option = opt;
	}

	void EndGame () {
		IsGameEnd = true;
	}

	public bool GetEndGame () {
		return IsGameEnd;
	}

	void SetUnlock()
	{
		lockType = LOCK_TYPE.FREE;
		yourID = "";
	}
	void SetLock()
	{
		lockType = LOCK_TYPE.LOCK;
		yourID = ((Entry)entryList[entryIdList[option[4]]]).id;
	}
	void SetLocked(string id)
	{
		lockType = LOCK_TYPE.LOCKED;
		yourID = id;
	}
	
	void UpdateEntryList(OMS.Entry[] list, bool isLock)
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
		if(entryList.Contains(yourID)) {
			return (yourID == "")? "" : ((Entry)entryList[yourID]).name;
		} else {
			Debug.Log("Unfind yourID!");
			return "";
		}
	}

	public string GetMyName ()
	{
		if(entryList.Contains(myID)) {
			return (myID == "")? "" : ((Entry)entryList[myID]).name;
		} else {
			Debug.Log("Unfind myID!");
			return "";
		}
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
