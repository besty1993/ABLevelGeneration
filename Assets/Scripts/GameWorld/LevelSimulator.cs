﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;


public class LevelSimulator {

	public static bool generateLevel = true;
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
		
		float tempX = subset.triggerX;
		float tempY = subset.triggerY;

		subset.triggerX = x;
		subset.triggerY = y;
		foreach (OBjData gameObj in subset.pigs) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			GameObject block = ABGameWorld.Instance.AddBlock(ABWorldAssets.BLOCKS[gameObj.type], pos,  rotation);
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), gameObj.material);
			block.GetComponent<ABBlock> ().SetMaterial (material);
		}

		foreach(PlatData gameObj in subset.platforms) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
            ABGameWorld.Instance.AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);
   		}

		foreach(OBjData gameObj in subset.tnts) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddBlock(ABWorldAssets.TNT, pos, rotation);
		}
	}

	public static void GenerateSubset (ABLevel subset,int tag, float x, float y) {

		float tempX = subset.triggerX;
		float tempY = subset.triggerY;

		subset.triggerX = x;
		subset.triggerY = y;
		foreach (OBjData gameObj in subset.pigs) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			GameObject temp = ABWorldAssets.PIGS [gameObj.type];
			temp.tag = tag.ToString ();
			ABGameWorld.Instance.AddPig(temp, pos, rotation);
		}

		foreach(BlockData gameObj in subset.blocks) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			GameObject temp = ABWorldAssets.BLOCKS[gameObj.type];
			temp.tag = tag.ToString ();
			GameObject block = ABGameWorld.Instance.AddBlock(temp, pos,  rotation);
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), gameObj.material);
			block.GetComponent<ABBlock> ().SetMaterial (material);
		}

		foreach(PlatData gameObj in subset.platforms) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			GameObject temp = ABWorldAssets.PLATFORM;
			temp.tag = tag.ToString ();
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			ABGameWorld.Instance.AddPlatform(temp, pos, rotation, gameObj.scaleX, gameObj.scaleY);
		}

		foreach(OBjData gameObj in subset.tnts) {
			gameObj.x += x-tempX;
			gameObj.y += y-tempY;
			Vector2 pos = new Vector2 (gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler (0, 0, gameObj.rotation);
			GameObject temp = ABWorldAssets.TNT;
			temp.tag = tag.ToString ();
			ABGameWorld.Instance.AddBlock(temp, pos, rotation);
		}
	}

	public static void DestroySubsetByTag (int tag) {
		Transform _trans = GameObject.FindGameObjectWithTag (tag.ToString ()).transform;
		foreach (Transform t in _trans) {
			Object.Destroy (t.gameObject);
		}
	}
//
//    //start Horizontal test
//    public void SubsetSimulationHorizontal(bool _isSimulation)
//    {
//        if (!_isSimulation || !CurrentGameWorld.IsLevelStable())
//            return;
//
//        if (!AlreadyDropHorizontal)
//        {
//            float y = HorizonalBulletPosition * 0.31f + ABGameWorld.platformStartPointY + 0.5f;
//            float x = ABGameWorld.platformStartPointX - 0.62f;
//
//            Vector2 pos = new Vector2(x, y);
//            Vector2 force = new Vector2(2, 0);
//            Quaternion rotation = Quaternion.Euler(0, 0, 0);
//
//            GameObject block = CurrentGameWorld.AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], pos, rotation);
//            MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
//            block.GetComponent<ABBlock>().SetMaterial(material);
//            block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
//            HorizonalBulletPosition++;
//            block.tag = "test";
//
//            if (HorizonalBulletPosition > HorizontalTimes)
//            {
//                HorizonalBulletPosition = 0;
//                HorizontalEvaluationStart = true;
//                CurrentGameWorld.initx();
//                return;
//            }
//            AlreadyDropHorizontal = true;
//        }
//        else
//        {
//            CurrentGameWorld.initx();
//            AlreadyDropHorizontal = false;
//        }
//
//    }
//
//    //start Vertical test
//    public void SubsetSimulationVertical()
//    {
//        if (!CurrentGameWorld._isSimulation || !CurrentGameWorld.IsLevelStable())
//            return;
//
//        if (!AlreadyDropVertical)
//        {
//            VerticalTimes = CurrentLevel.platforms.Count + 1;
//            float x = VerticalBulletPosition * 0.62f + ABGameWorld.platformStartPointX - 0.32f;
//            float y = ABGameWorld.platformStartPointY + 5f;
//            Vector2 pos = new Vector2(x, y);
//            Vector2 force = new Vector2(0, -2f);
//            Quaternion rotation = Quaternion.Euler(0, 0, 0);
//            GameObject block = CurrentGameWorld.AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], pos, rotation);
//            block.tag = "test";
//            block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
//            MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
//            block.GetComponent<ABBlock>().SetMaterial(material);
//            VerticalBulletPosition++;
//            if (VerticalBulletPosition > VerticalTimes)
//            {
//                VerticalBulletPosition = 0;
//                HorizontalEvaluationStart = false;
//                CurrentGameWorld.NextLevel();
//                return;
//            }
//
//            AlreadyDropVertical = true;
//        }
//        else
//        {
//            CurrentGameWorld.initx();
//            AlreadyDropVertical = false;
//        }
//        //		if (IsLevelStable ()&&block.transform.position.y<5) {
//        //			initx ();
//        //		}
//
//    }
//    //check the subset can be used or not
//    public void CheckUseful(Transform _blocksTransform)
//    {
//        foreach (Transform b in _blocksTransform)
//        {
//            if ((b.tag != "test") && b.position.y < ABGameWorld.platformStartPointY - 0.5f)
//            {
//                CurrentLevel.usefulLevel = true;
//                CurrentGameWorld.NextLevel();
//            }
//        }
//    }
//
//    public void LevelShaking(Transform _blocksTransform)
//    {
//        if (_blocksTransform.childCount == 0)
//            return;
//        foreach (Transform block in _blocksTransform)
//        {   
//            if (block.GetComponent<Rigidbody2D>().velocity.magnitude > 0.3f)
//            {
//                CurrentLevel.levelShaking = true;
//                SaveLevel.SaveLevel(CurrentLevel, false);
//                CurrentLevel.usefulLevel = false;
//            }
//
//        }
//    }

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

//
//
//	public static void run_cmd(string cmd, string args)
//	{
//		ProcessStartInfo start = new ProcessStartInfo();
//		start.FileName = "PATH_TO_PYTHON_EXE";
//		start.Arguments = string.Format("\"{0}\" \"{1}\"", cmd, args);
//		start.UseShellExecute = false;// Do not use OS shell
//		start.CreateNoWindow = false; // We don't need new window
//		start.RedirectStandardOutput = true;// Any output, generated by application will be redirected back
//		start.RedirectStandardError = true; // Any error in standard output will be redirected back (for example exceptions)
////		using (Process process = Process.Start(start))
////		{
////			using (StreamReader reader = process.StandardOutput)
////			{
////				string stderr = process.StandardError.ReadToEnd(); // Here are the exceptions from our Python script
////				string result = reader.ReadToEnd(); // Here is the result of StdOut(for example: print "test")
////				return result;
////			}
////		}
//	}
//
	public static void RunSubsetGenerator() {
		// using System.Diagnostics;
		UnityEngine.Debug.Log("Running Subset Generator...");
		#if UNITY_STANDALONE_OSX
		Process p = new Process();
		p.StartInfo.FileName = "python";
		p.StartInfo.Arguments = "subset_generator.py";    
		// Pipe the output to itself - we will catch this later
		p.StartInfo.RedirectStandardError=true;
		p.StartInfo.RedirectStandardOutput=true;
		p.StartInfo.CreateNoWindow = true;

		// Where the script lives
		p.StartInfo.WorkingDirectory = Application.streamingAssetsPath+"/Subsets/"; 
		p.StartInfo.UseShellExecute = false;

		p.Start();
		// Read the output - this will show is a single entry in the console - you could get  fancy and make it log for each line - but thats not why we're here
		//		UnityEngine.Debug.Log( p.StandardOutput.ReadToEnd() );
		p.WaitForExit();
		p.Close();
		#elif UNITY_STANDALONE_WIN
		Process p = new Process();
		p.StartInfo.FileName = "python";
		p.StartInfo.Arguments = "subset_generator.py";    
		// Pipe the output to itself - we will catch this later
		p.StartInfo.RedirectStandardError=true;
		p.StartInfo.RedirectStandardOutput=true;
		p.StartInfo.CreateNoWindow = true;

		// Where the script lives
		p.StartInfo.WorkingDirectory = Application.streamingAssetsPath+"/Subsets/"; 
		p.StartInfo.UseShellExecute = false;

		p.Start();
		// Read the output - this will show is a single entry in the console - you could get  fancy and make it log for each line - but thats not why we're here
		//		UnityEngine.Debug.Log( p.StandardOutput.ReadToEnd() );
		p.WaitForExit();
		p.Close();
		#endif
	}
}