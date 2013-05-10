using UnityEngine;
using System.Collections;
using Othello;

public class PieceObject
{
	GameObject blackSide;
	GameObject whiteSide;
	bool enabled;
	Vector3 position;
	Quaternion rotation;
	float angle;
	float hight;
	
	const float OFFSETY = 0.05f;
	
	public PieceObject(ref GameObject blackPiece, ref GameObject whitePiece, Vector3 pos)
	{
		blackSide = blackPiece;
		blackSide.renderer.material.color = new Color(0,0,0,255);
		
		whiteSide = whitePiece;
		whiteSide.renderer.material.color = new Color(255,255,255,255);

		Enabled(false);
		
		angle = 0f;
		hight = 0f;
		position = pos;
		CalcTransform();
	}

	public void Enabled(bool enableFlag) 
	{
		if(blackSide && whiteSide)
		{
			enabled = enableFlag;
			blackSide.renderer.enabled = enabled;
			whiteSide.renderer.enabled = enabled;
		}
	}

	public bool GetEnabled() 
	{
		return enabled;
	}
	
	public void SetHight(float v)
	{
		hight = v;
	}
	
	public void ToBlack(bool animationFlag = true)
	{
		if(animationFlag)
		{
			angle -= 10f * 60f * Time.deltaTime;
			if(angle < 0f) angle = 0f;
		}
		else
		{
			angle = 0f;
		}	

		if(hight > 0f)
		{
			hight -= hight*0.5f * 60f * Time.deltaTime;
		}
		
		CalcTransform();
	}

	public void ToWhite(bool animationFlag = true)
	{
		if(animationFlag)
		{
			angle += 10f * 60f * Time.deltaTime;
			if(angle > 180f) angle = 180f;
		}
		else
		{
			angle = 180f;
		}	

		if(hight > 0f)
		{
			hight -= hight*0.5f * 60f * Time.deltaTime;
		}
		
		CalcTransform();
	}

	void CalcTransform()
	{
		rotation = Quaternion.AngleAxis(angle, new Vector3(0,0,1));

		blackSide.transform.rotation = rotation;
		blackSide.transform.position = position;
		blackSide.transform.position += new Vector3(0f, hight, 0f);
		blackSide.transform.position += rotation * new Vector3(0f, OFFSETY, 0f);

		whiteSide.transform.rotation = rotation;
		whiteSide.transform.position = position;
		whiteSide.transform.position += new Vector3(0f, hight, 0f);
		whiteSide.transform.position -= rotation * new Vector3(0f, OFFSETY, 0f);
	}
}
	
