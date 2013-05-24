using UnityEngine;
using System.Collections;
using Othello;

public class board : MonoBehaviour
{
	// Prefab定義/
	public GameObject boardPrefab;
	public GameObject guidePrefab;
	public GameObject markerPrefab;
	
	// 盤面のオブジェクト/
	GameObject boardObject;
	
	// 盤面のガイド表示のオブジェクトリスト/
	GameObject[,] guide = new GameObject[2,8];
	// 盤面のガイド表示用文字データ/
	string[,] guideLitteral = new string[2,8]{
		{"a","b","c","d","e","f","g","h"},
		{"1","2","3","4","5","6","7","8"}
	};

	// 盤面に配置した着手可能場所マーカーのオブジェクトリスト/
	ArrayList markerList = new ArrayList();

	// メニューコンポーネントキャッシュ用/
	menu compMenu = null;
	// メインコンポーネントキャッシュ用/
	main compMain = null;
	
	void Awake () {
		compMenu = GameObject.Find("Menu").GetComponent<menu>();
		compMain = GetComponent<main>();
	}
	
	// Use this for initialization
	void Start () {
	
		// 盤面オブジェクト生成/
		boardObject = (GameObject)Instantiate(boardPrefab, new Vector3(0,0,0), Quaternion.identity);
		
		// 盤面の横ガイド表示用オブジェクト生成/
		float x=-3.5f,y=+4.5f;
		for (int i=0; i<8; i++) {
			guide[0,i] = (GameObject)Instantiate(guidePrefab, new Vector3(0,0,0), Quaternion.identity);
			guide[0,i].guiText.text = guideLitteral[0,i];
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[0,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			guide[0,i].guiText.fontSize = 20;
			guide[0,i].guiText.anchor = TextAnchor.MiddleCenter;
			guide[0,i].guiText.alignment = TextAlignment.Center;
			x+=1f;
		}
		// 盤面の縦ガイド表示用オブジェクト生成/
		x=-4.5f;y=+3.5f;
		for (int i=0; i<8; i++) {
			guide[1,i] = (GameObject)Instantiate(guidePrefab, new Vector3(0,0,0), Quaternion.identity);
			guide[1,i].guiText.text = guideLitteral[1,i];
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[1,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			guide[1,i].guiText.fontSize = 20;
			guide[1,i].guiText.anchor = TextAnchor.MiddleCenter;
			guide[1,i].guiText.alignment = TextAlignment.Center;
			y-=1f;
		}
	}

	// Update is called once per frame
	void Update () {
		byte alfa;
		alfa = (Time.time % 1.0f < 0.5f)? (byte)((Time.time % 1.0f) * 255 + 127) : (byte)((1.0f - Time.time % 1.0f) * 255 + 127);
		
		// マーカー明滅/
		foreach(GameObject obj in markerList) {
			if(obj) {
				obj.renderer.material.color = new Color32(209,221, 48,alfa);
			}
		}
		
		// ガイド位置更新/
		float x=-3.5f,y=+4.5f;
		for (int i=0; i<8; i++) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[0,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			x+=1f;
		}
		x=-4.5f;y=+3.5f;
		for (int i=0; i<8; i++) {
			Vector3 screenPos = Camera.main.WorldToScreenPoint(new Vector3(x,0f,y));
			guide[1,i].transform.position = new Vector3(screenPos.x/Screen.width,screenPos.y/Screen.height,0);
			y-=1f;
		}
		
	}
	
	void OnDestroy()
	{
		// 盤面のオブジェクト削除/
		GameObject.Destroy(boardObject);

		// 盤面のガイドオブジェクト削除/
		for (int i=0; i<8; i++) {
			GameObject.Destroy(guide[0,i]);
		}
		for (int i=0; i<8; i++) {
			GameObject.Destroy(guide[1,i]);
		}
		
		// マーカーのオブジェクト削除/
		foreach(Object obj in markerList) {
			GameObject.Destroy(obj);
		}
	}
	
	public void MarkerEnabled(bool enableFlag)
	{
		foreach(GameObject obj in markerList) {
			if(obj) {
				obj.renderer.enabled = enableFlag;
			}
		}
	}	

	public void MarkerUpdate()
	{
		// 着手可能場所マーカーのオブジェクトを削除/
		foreach(Object obj in markerList) {
			GameObject.Destroy(obj);
		}
		// 着手可能場所チェック/
		ArrayList PutableList = new ArrayList();
		if (!Board.Instance().CheckPutable(compMain.GetPieceSide(), ref PutableList)) {
			// 攻守交代2回してどこも置けなかったらそのゲームは終了/
			if (!Board.Instance().CheckPutable(compMain.ChangePieceSide(), ref PutableList)) {
				compMain.SetGameEnd(main.GAME_STATUS.GameOver);
			}
		}
		// 着手可能場所マーカーのオブジェクトを生成/
		if (compMenu.GetGuideEnable() && !compMain.IsAI()) {
			foreach(Board.Position v in PutableList) {
				Vector3 position = new Vector3(v.x -4.0f + 0.5f, 0.3f, v.y -4.0f + 0.5f);
				markerList.Add(Instantiate(markerPrefab, position, Quaternion.identity));
			}
		}
		PutableList.Clear();
	}
}
	
