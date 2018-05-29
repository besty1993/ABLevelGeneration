﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelSimulator
{
	public LevelSimulator ()
	{
	}


	public static void SetSubsetTrigger (ABLevel subset, float x, float y) {
		subset.triggerX = x;
		subset.triggerY = y;
	}

	//Change Subset's position into different position.
	public static void ChangeSubsetPosition (ABLevel subset, float x, float y) {
		float tempX = subset.triggerX;
		float tempY = subset.triggerY;
		subset.triggerX = x;
		subset.triggerY = y;
		foreach (OBjData gameObj in subset.pigs) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
		}

		foreach(BlockData gameObj in subset.blocks) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
		}

		foreach(PlatData gameObj in subset.platforms) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
		}

		foreach(OBjData gameObj in subset.tnts) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
		}
	}

	public static void GenerateSubset (ABLevel subset) {
		foreach (OBjData gameObj in subset.pigs) {

			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {

			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);

			GameObject block = ABGameWorld.Instance.AddBlock(ABWorldAssets.BLOCKS[gameObj.type], pos,  rotation);

			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), gameObj.material);
			block.GetComponent<ABBlock> ().SetMaterial (material);
		}

		foreach(PlatData gameObj in subset.platforms) {
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);

			ABGameWorld.Instance.AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);
		}

		foreach(OBjData gameObj in subset.tnts) {

			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);

			ABGameWorld.Instance.AddBlock(ABWorldAssets.TNT, pos, rotation);
		}
		
	}

	public void DestroySubset (ABLevel subset) {
		foreach (OBjData obj in subset.pigs) {
			UnityEngine.Object.Destroy (obj.gameObj);
		}

		foreach(BlockData obj in subset.blocks) {
			UnityEngine.Object.Destroy (obj.gameObj);
		}

		foreach(PlatData obj in subset.platforms) {
			UnityEngine.Object.Destroy (obj.gameObj);
		}

		foreach(OBjData obj in subset.tnts) {
			UnityEngine.Object.Destroy (obj.gameObj);
		}
	}
}