using UnityEngine;
using System.Collections;

[System.Serializable]
public class Config 
{
	public string name = "name";
	public string aiName = "ai_name";
	public string iconFile = "hoge.png";

}

public class menu : MonoBehaviour
{
	public Config configData;
	
	Rect[] windowRect = {
		new Rect (10, 60, 200, 20),
		new Rect (10, 160, 200, 20),
		new Rect (220, 60, 200, 20),
		new Rect (220, 160, 200, 20),
		new Rect (440, 60, 300, 20)
	};
	int[] option = {0,0,0,0,0};
	float[] timeTable = {
		0f, 5f, 10f, 15f, 20f, 25f, 30f
	};
		
	string[] entryList = {};
	
	void OnGUI ()
	{
		windowRect[0] = GUILayout.Window (0, windowRect[0], MakeSelectWindow, StringTable.SENTE);
		windowRect[1] = GUILayout.Window (1, windowRect[1], MakeSelectWindow, StringTable.GOTE);
		windowRect[2] = GUILayout.Window (2, windowRect[2], MakeGuideWindow, StringTable.GUIDE);
		windowRect[3] = GUILayout.Window (3, windowRect[3], MakeTimeWindow, StringTable.TIME);
		windowRect[4] = GUILayout.Window (4, windowRect[4], MakeEntryWindow, StringTable.ENTRY);

		GUILayout.BeginArea( new Rect (10, 10, 410, 40));
			GUILayout.Space(10);
			if(GUILayout.Button(StringTable.START)) {
				this.enabled = false;
				Application.LoadLevel("Main");
		    }
		GUILayout.EndArea();	
	}
	
	void Awake ()
	{
		DontDestroyOnLoad (this);
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
		GUILayout.Space (10);
		if (entryList.GetLength(0) > 0 ) {
			option[id] = GUILayout.SelectionGrid(option[id], entryList, 1);
		}
		GUILayout.FlexibleSpace ();
	}

	public void SetEntry(Entry[] list)
	{
		entryList = new string[list.Length+1];
		entryList[0] = StringTable.NO_VS;
		for(int i=0; i<list.Length; i++) {
			entryList[i+1] = list[i].name;
		}
	}
	
	public bool getGuideEnable () 
	{
		return (option[2] == 0);
	}

	public float getLimitTime () 
	{
		return timeTable[option[3]];
	}
}
