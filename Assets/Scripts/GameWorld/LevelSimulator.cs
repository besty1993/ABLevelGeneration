using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class LevelSimulator {

    public static bool Generatelevel = true;
    public static float platformSize = 0.62f;
    public static int platformCount = 0;
    public static float firstplatformX;
    public static float firstplatformY;
	// False : Evaluate Subset	True : Generate Level

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

    public static void GenerateSubset (ABLevel subset, float x, float y) {
        
        foreach (PlatData gameObj in subset.platforms)
        {
            if (platformCount == 0)
            {
                firstplatformX = gameObj.x;
                firstplatformY = gameObj.y;
            }
            //Debug.Log(gameObj.x + ", " + gameObj.y + ", " + x + ", " + y);
            Vector2 pos = new Vector2(Mathf.Abs((Mathf.Abs(firstplatformX) - Mathf.Abs(gameObj.x))) + x, (Mathf.Abs(Mathf.Abs(firstplatformY) - Mathf.Abs(gameObj.y)) + y));
            Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);
            platformCount++;
        }

        foreach (OBjData gameObj in subset.tnts)
        {
            Vector2 pos = new Vector2(Mathf.Abs((Mathf.Abs(firstplatformX) - Mathf.Abs(gameObj.x))) + x-0.2f, (Mathf.Abs(Mathf.Abs(firstplatformY) - Mathf.Abs(gameObj.y)) + y));
            //Debug.Log(gameObj.type + ", " + pos + ", " + gameObj.x + ", " + x + ", " + gameObj.y + ", " + y);
            Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddBlock(ABWorldAssets.TNT, pos, rotation);
        }

		foreach (OBjData gameObj in subset.pigs) {
            Vector2 pos = new Vector2(Mathf.Abs((Mathf.Abs(firstplatformX) - Mathf.Abs(gameObj.x))) + x, (Mathf.Abs(Mathf.Abs(firstplatformY) - Mathf.Abs(gameObj.y)) + y));
            //Debug.Log(gameObj.type+", "+pos+", "+gameObj.x+", "+x+", "+gameObj.y+", "+y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {
            Vector2 pos = new Vector2(Mathf.Abs((Mathf.Abs(firstplatformX) - Mathf.Abs(gameObj.x))) + x, (Mathf.Abs(Mathf.Abs(firstplatformY) - Mathf.Abs(gameObj.y)) + y));
            //Debug.Log(gameObj.type + ", " + pos + ", " + gameObj.x + ", " + x + ", " + gameObj.y + ", " + y);
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