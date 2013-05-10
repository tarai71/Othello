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
	bool drop;
	
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
		drop = false;
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
		drop = true;
		hight = v;
	}
	
	public bool GetDrop()
	{
		return drop;
	}
	
	public void ToDrop()
	{
		hight -= hight*0.4f * (60f * Time.deltaTime);
		if(hight <= 0.01f)
		{
			drop = false;
			hight = 0f;
		}

		CalcTransform();
	}
	
	public void ToBlack(bool animationFlag = true)
	{
		if(animationFlag)
		{
			angle -= 10f * (60f * Time.deltaTime);
			if(angle < 0f) angle = 0f;
	
			if(angle < 90f) hight = angle * 1f / 90f;
			if(angle >= 90f) hight = (180f - angle) * 1f / 90f;
		}
		else
		{
			angle = 0f;
		}
		
		CalcTransform();
	}

	public void ToWhite(bool animationFlag = true)
	{
		if(animationFlag)
		{
			angle += 10f * (60f * Time.deltaTime);
			if(angle > 180f) angle = 180f;
			
			if(angle < 90f) hight = angle * 1f / 90f;
			if(angle >= 90f) hight = (180f - angle) * 1f / 90f;
		}
		else
		{
			angle = 180f;
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
	
