using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;


public class LevelSimulator {

	public static bool Generatelevel = false;
    public static int VerticalBulletPosition = 0;
    public static int VerticalTimes;
    public static int HorizonalBulletPosition = 0;
    public static int HorizontalTimes = 10;
    public static bool AlreadyDropHorizontal = false;
    public static bool AlreadyDropVertical = false;
    public static int BlockId;
    public static bool HorizontalEvaluationStart = false;
    // False : Evaluate Subset	True : Generate Level
    public ABLevel CurrentLevel;
    public LevelLoader SaveLevel;
    public ABGameWorld CurrentGameWorld;

    void Start()
    {
        CurrentLevel = LevelList.Instance.GetCurrentLevel();
        SaveLevel = new LevelLoader();
        CurrentGameWorld = new ABGameWorld();
    }

    
	public LevelSimulator () {  
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

    public static void GenerateSubset (ABLevel subset, float x, float y) {
		foreach (OBjData gameObj in subset.pigs) {
            Vector2 pos = new Vector2 (gameObj.x + x, gameObj.y + y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {
            Vector2 pos = new Vector2 (gameObj.x + x, gameObj.y + y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			GameObject block = ABGameWorld.Instance.AddBlock(ABWorldAssets.BLOCKS[gameObj.type], pos,  rotation);
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), gameObj.material);
			block.GetComponent<ABBlock> ().SetMaterial (material);
		}

		foreach(PlatData gameObj in subset.platforms) {
            Vector2 pos = new Vector2 (gameObj.x + x, gameObj.y + y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);
   		}

		foreach(OBjData gameObj in subset.tnts) {
            Vector2 pos = new Vector2 (gameObj.x + x, gameObj.y + y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddBlock(ABWorldAssets.TNT, pos, rotation);
		}
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

    //start Horizontal test
    public void SubsetSimulationHorizontal(bool _isSimulation)
    {
        if (!_isSimulation || !CurrentGameWorld.IsLevelStable())
            return;

        if (!AlreadyDropHorizontal)
        {
            float y = HorizonalBulletPosition * 0.31f + ABGameWorld.platformStartPointY + 0.5f;
            float x = ABGameWorld.platformStartPointX - 0.62f;

            Vector2 pos = new Vector2(x, y);
            Vector2 force = new Vector2(2, 0);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);

            GameObject block = CurrentGameWorld.AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], pos, rotation);
            MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
            block.GetComponent<ABBlock>().SetMaterial(material);
            block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            HorizonalBulletPosition++;
            block.tag = "test";

            if (HorizonalBulletPosition > HorizontalTimes)
            {
                HorizonalBulletPosition = 0;
                HorizontalEvaluationStart = true;
                CurrentGameWorld.initx();
                return;
            }
            AlreadyDropHorizontal = true;
        }
        else
        {
            CurrentGameWorld.initx();
            AlreadyDropHorizontal = false;
        }

    }

    //start Vertical test
    public void SubsetSimulationVertical()
    {
        if (!CurrentGameWorld._isSimulation || !CurrentGameWorld.IsLevelStable())
            return;

        if (!AlreadyDropVertical)
        {
            VerticalTimes = CurrentLevel.platforms.Count + 1;
            float x = VerticalBulletPosition * 0.62f + ABGameWorld.platformStartPointX - 0.32f;
            float y = ABGameWorld.platformStartPointY + 5f;
            Vector2 pos = new Vector2(x, y);
            Vector2 force = new Vector2(0, -2f);
            Quaternion rotation = Quaternion.Euler(0, 0, 0);
            GameObject block = CurrentGameWorld.AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], pos, rotation);
            block.tag = "test";
            block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
            MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
            block.GetComponent<ABBlock>().SetMaterial(material);
            VerticalBulletPosition++;
            if (VerticalBulletPosition > VerticalTimes)
            {
                VerticalBulletPosition = 0;
                HorizontalEvaluationStart = false;
                CurrentGameWorld.NextLevel();
                return;
            }

            AlreadyDropVertical = true;
        }
        else
        {
            CurrentGameWorld.initx();
            AlreadyDropVertical = false;
        }
        //		if (IsLevelStable ()&&block.transform.position.y<5) {
        //			initx ();
        //		}

    }
    //check the subset can be used or not
    public void CheckUseful(Transform _blocksTransform)
    {
        foreach (Transform b in _blocksTransform)
        {
            if ((b.tag != "test") && b.position.y < ABGameWorld.platformStartPointY - 0.5f)
            {
                CurrentLevel.usefulLevel = true;
                CurrentGameWorld.NextLevel();
            }
        }
    }

    public void LevelShaking(Transform _blocksTransform)
    {
        if (_blocksTransform.childCount == 0)
            return;
        foreach (Transform block in _blocksTransform)
        {   
            if (block.GetComponent<Rigidbody2D>().velocity.magnitude > 0.3f)
            {
                CurrentLevel.levelShaking = true;
                SaveLevel.SaveLevel(CurrentLevel, false);
                CurrentLevel.usefulLevel = false;
            }

        }
    }

    //public bool CheckTNT()
    //{
    //    if (Check.tnts.Count != 0)
    //    {
    //        GameObject[] tntCount = GameObject.FindGameObjectsWithTag("TNT");
    //        if ( tntCount.Length== 0)
    //            return false;
    //        else
    //        {
    //            Check.triggerX = tntCount[0].transform.position.x;
    //            Check.triggerY = tntCount[0].transform.position.y;
    //            Check.usefulLevel = true;
    //            return true;
    //        }
    //    }
    //    else
    //    {
    //        return false;
    //    }
    //}

    //LevelShaking check if the generated is stable



    //CheckShaking();
    //if (IsLevelStable())
    //{
    //    NextLevel();
    //}
    //public void CheckShaking()
    //{
    //    foreach (Transform block in _blocksTransform) {
    //        if (block.GetComponent<Rigidbody2D>().velocity.magnitude > LevelMaxSpeed)
    //            LevelMaxSpeed = block.GetComponent<Rigidbody2D>().velocity.magnitude;
    //    }
    //    StreamWriter recordLevel = new StreamWriter(System.Environment.CurrentDirectory + "/Assets/StreamingAssets/Levels/levelcheck.txt", true);
    //    recordLevel.WriteLine((LevelList.Instance.CurrentIndex).ToString()+":"+LevelMaxSpeed);
    //    recordLevel.Close();
    //}



}