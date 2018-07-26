using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelSimulator {

    public static bool GenerateLevel = true;
    public static float platformSize = 0.62f;
    public static int platformCount = 0;
    public static Vector2 firstPlatformPosition;

    public static float rangeTriggerPoint;
	// False : Evaluate Subset	True : Generate Level

	public static void SetSubsetTrigger (ABLevel subset, float x, float y) {
		subset.triggerX = x;
		subset.triggerY = y;
	}

	//Change Subset's position into different position.
	public static void ChangeSubsetPosition (ABLevel subset, float x, float y) {
        if (platformCount == 0)
        {
            
            firstPlatformPosition.x = subset.triggerX;
            firstPlatformPosition.y = subset.triggerY;
        }

        foreach (PlatData gameObj in subset.platforms)
        {
            gameObj.x = (firstPlatformPosition.x + gameObj.x) + x;
            gameObj.y = (firstPlatformPosition.y + gameObj.y) + y;
            platformCount++;
        }

		foreach (OBjData gameObj in subset.pigs) {
            gameObj.x = (firstPlatformPosition.x + gameObj.x) + x;
            gameObj.y = (firstPlatformPosition.y + gameObj.y) + y;
		}

		foreach(BlockData gameObj in subset.blocks) {
            gameObj.x = (firstPlatformPosition.x + gameObj.x) + x;
            gameObj.y = (firstPlatformPosition.y + gameObj.y) + y;
		}

		foreach(OBjData gameObj in subset.tnts) {
            gameObj.x = (firstPlatformPosition.x + gameObj.x) + x;
            gameObj.y = (firstPlatformPosition.y + gameObj.y) + y;
		}
        platformCount = 0;
	}

    public static void GenerateSubset (ABLevel subset, float x, float y) {



        foreach (PlatData gameObj in subset.platforms)
        {
            if (platformCount == 0)
            {
                firstPlatformPosition.x = gameObj.x;
                firstPlatformPosition.y = gameObj.y;
                //Debug.Log("LLKKK "+subset.triggerX);
                // subset.triggerX subset.triggerY
                foreach (OBjData gameObjTNT in subset.tnts)
                {
                    rangeTriggerPoint = (Mathf.Abs(firstPlatformPosition.x) - Mathf.Abs(gameObjTNT.x)) + 0.5F;
                    //Debug.Log("firstPlatformPosition.x " + firstPlatformPosition.x);
                    //Debug.Log("gameObjTNT.x " + gameObjTNT.x);
                    //Debug.Log("rangeTriggerPoint " + rangeTriggerPoint);
                }
            }
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(firstPlatformPosition.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint, (Mathf.Abs(Mathf.Abs(firstPlatformPosition.y) - Mathf.Abs(gameObj.y)) + y));
            Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);
            platformCount++;
        }

        foreach (OBjData gameObj in subset.tnts)
        {
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(firstPlatformPosition.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint, (Mathf.Abs(Mathf.Abs(firstPlatformPosition.y) - Mathf.Abs(gameObj.y)) + y));
            Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddBlock(ABWorldAssets.TNT, pos, rotation);
        }

		foreach (OBjData gameObj in subset.pigs) {
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(firstPlatformPosition.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint, (Mathf.Abs(Mathf.Abs(firstPlatformPosition.y) - Mathf.Abs(gameObj.y)) + y));
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {
            Vector2 pos = new Vector2((Mathf.Abs((Mathf.Abs(firstPlatformPosition.x) - Mathf.Abs(gameObj.x))) + x) - rangeTriggerPoint, (Mathf.Abs(Mathf.Abs(firstPlatformPosition.y) - Mathf.Abs(gameObj.y)) + y));
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