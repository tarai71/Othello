using UnityEngine;
using System.Collections;

public class config : MonoBehaviour
{
	INIFile file;
	
	string myname;
	string ainame;
	
	public string MyName
	{
		get { return myname; }
	}

	public string AiName
	{
		get { return ainame; }
	}
	
	void Awake ()
	{
		DontDestroyOnLoad (this);
	}

	// Use this for initialization
	void Start ()
	{
		file = new INIFile("./othello.ini");
		Check();
	}
	
	void Update ()
	{
	}
	
	void OnGUI ()
	{
		GUILayout.BeginArea( new Rect (10, 10, 250, 400));
		GUILayout.Space(10);

		GUILayout.BeginHorizontal();
		GUILayout.Label(StringTable.NAME + ":", GUILayout.MinWidth (90));
		myname = GUILayout.TextField(myname, 16, GUILayout.MinWidth (150));
		GUILayout.EndHorizontal();
			
		GUILayout.BeginHorizontal();
		GUILayout.Label(StringTable.AINAME + ":", GUILayout.MinWidth (90));
		ainame = GUILayout.TextField(ainame, 16, GUILayout.MinWidth (150));
		GUILayout.EndHorizontal();
		
		if(GUILayout.Button(StringTable.REGIST))
		{
		 	file["config","name"] = myname;
		 	file["config","ainame"] = ainame;
			Check();
		}
		GUILayout.EndArea();	
	}
	
	void Check()
	{
		myname = file["config","name"];
		ainame = file["config","ainame"];
		if(name != "" && ainame != "")
		{
			Debug.Log(StringTable.NAME + ":" + myname);
			Debug.Log(StringTable.AINAME + ":" + ainame);
			this.enabled = false;
			Application.LoadLevel("Menu");
		}
	}
}
