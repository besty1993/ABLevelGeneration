using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelSimulator {

    public static bool GenerateLevel = true;
    public static float platformSize = 0.62f;
    public static Vector2 platformStartPoint;
    public static Vector2 rangeTriggerPoint;
	// False : Evaluate Subset	True : Generate Level

	public static void SetSubsetTrigger (ABLevel subset, float x, float y) {
        subset.triggerX = subset.triggerX + x;
        subset.triggerY = subset.triggerY + y;
	}

	//Change Subset's position into different position.
	public static void ChangeSubsetPosition (ABLevel subset, float x, float y) {
        foreach (PlatData gameObj in subset.platforms)
        {
            gameObj.x = (gameObj.x) + x;
            gameObj.y = (gameObj.y) + y;
        }

		foreach (OBjData gameObj in subset.pigs) {
            gameObj.x = (gameObj.x) + x;
            gameObj.y = (gameObj.y) + y;
		}

		foreach(BlockData gameObj in subset.blocks) {
            gameObj.x = (gameObj.x) + x;
            gameObj.y = (gameObj.y) + y;
		}

		foreach(OBjData gameObj in subset.tnts) {
            gameObj.x = (gameObj.x) + x;
            gameObj.y = (gameObj.y) + y;
		}
	}

    public static void GenerateSubset (ABLevel subset, float x, float y) {
        int platformCount = 0;
        foreach (PlatData gameObj in subset.platforms)
        {
            if (platformCount == 0)
            {
                platformStartPoint.x = gameObj.x;
                platformStartPoint.y = gameObj.y;
                rangeTriggerPoint.x = (Mathf.Abs(platformStartPoint.x) - Mathf.Abs(subset.triggerX));
                rangeTriggerPoint.y = (Mathf.Abs(platformStartPoint.y) - Mathf.Abs(subset.triggerY));
                //Debug.Log("rangeTriggerPoint "+rangeTriggerPoint);
            }
            //Debug.Log("gameObj.x pos " + Mathf.Abs(gameObj.x));
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(platformStartPoint.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint.x, (Mathf.Abs(Mathf.Abs(platformStartPoint.y) - Mathf.Abs(gameObj.y)) + y) - rangeTriggerPoint.y);
            Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);
            platformCount++;
            //Debug.Log("platform pos "+pos);
        }

        foreach (OBjData gameObj in subset.tnts)
        {
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(platformStartPoint.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint.x, (Mathf.Abs(Mathf.Abs(platformStartPoint.y) - Mathf.Abs(gameObj.y)) + y) - rangeTriggerPoint.y);
            Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddBlock(ABWorldAssets.TNT, pos, rotation);
        }

		foreach (OBjData gameObj in subset.pigs) {
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(platformStartPoint.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint.x, (Mathf.Abs(Mathf.Abs(platformStartPoint.y) - Mathf.Abs(gameObj.y)) + y) - rangeTriggerPoint.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(platformStartPoint.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint.x, (Mathf.Abs(Mathf.Abs(platformStartPoint.y) - Mathf.Abs(gameObj.y)) + y) - rangeTriggerPoint.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			GameObject block = ABGameWorld.Instance.AddBlock(ABWorldAssets.BLOCKS[gameObj.type], pos,  rotation);
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), gameObj.material);
			block.GetComponent<ABBlock> ().SetMaterial (material);
		}
        platformCount = 0;
	}

    public static void DestroySubset (ABLevel subset) {
		foreach (OBjData obj in subset.pigs) {
            Object.DestroyImmediate(obj.gameObj, true);
		}

		foreach(BlockData obj in subset.blocks) {
            Object.DestroyImmediate(obj.gameObj, true);
		}

		foreach(PlatData obj in subset.platforms) {
            Object.DestroyImmediate(obj.gameObj, true);
		}

		foreach(OBjData obj in subset.tnts) {
            Object.DestroyImmediate(obj.gameObj, true);
		}
	}
}