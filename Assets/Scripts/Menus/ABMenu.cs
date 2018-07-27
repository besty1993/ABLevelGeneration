// SCIENCE BIRDS: A clone version of the Angry Birds game used for 
// research purposes
// 
// Copyright (C) 2016 - Lucas N. Ferreira - lucasnfe@gmail.com
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>
//

﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class ABMenu : MonoBehaviour {
	/// <summary>
	/// This Start() is activating GameWolrd Scene directly
	/// Always start from level 1.
	/// </summary>
	public static bool finished = false; //If it's true, Level Generater ends.
	public static List<string> parameters;

	void Start() {
		if (finished) {
			Application.Quit ();
			return;
		}

		SwitchSubsetAndLevel ();

	}

	public void SwitchSubsetAndLevel () {
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name != "GameWorld") {
			if (!LevelSimulator.Generatelevel) {
				DeleteSubsets ();

				StreamReader sr = new StreamReader (System.Environment.CurrentDirectory + "/parameters.txt", true);
				string temp = sr.ReadToEnd ();
				parameters = new List<string> (temp.Split ('\n'));
				sr.Close ();

				StreamWriter sw = new StreamWriter (Application.streamingAssetsPath + "/Subsets/parameters.txt", true);
				sw.WriteLine (parameters [0]);
				sw.WriteLine (parameters [1]);
				sw.WriteLine (parameters [2]);
				sw.Close ();

				parameters.RemoveRange (0, 3);

				LevelSimulator.RunSubsetGenerator ();
				LoadNextScene ("LevelSelectMenu", false, null);
			} else {
				LoadNextScene ("LevelSelectMenu", false, null);
			}
		}
	}


	public void LoadNextScene(string sceneName) {

		ABSceneManager.Instance.LoadScene(sceneName);
	}

	public void LoadNextScene(string sceneName, bool loadTransition, ABSceneManager.ActionBetweenScenes action) {

		ABSceneManager.Instance.LoadScene(sceneName, loadTransition, action);
	}

	public void DeleteSubsets () {
//		#if UNITY_EDITOR
//		UnityEditor.FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath + "/Subsets/parameters.txt");
//		UnityEditor.FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath + "/EvaluatedSubsets/*.xml");
//		UnityEditor.FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath + "/Subsets/*.xml");
//		print("Unity Editor 32");
//		#elif UNITY_EDITOR_64
//		UnityEditor.FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath + "/Subsets/parameters.txt");
//		UnityEditor.FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath + "/EvaluatedSubsets/*.xml");
//		UnityEditor.FileUtil.DeleteFileOrDirectory (Application.streamingAssetsPath + "/Subsets/*.xml");
//		print("Unity Editor 64");
//		#else
		foreach (string file in Directory.GetFiles(Application.streamingAssetsPath+"/Subsets/","*.xml",SearchOption.AllDirectories))
			File.Delete (file);
		foreach (string file in Directory.GetFiles(Application.streamingAssetsPath+"/EvaluatedSubsets/","*.xml",SearchOption.AllDirectories))
			File.Delete (file);
		File.Delete (Application.streamingAssetsPath + "/Subsets/parameters.txt");
		print("No Unity Editor");
//		#endif
	}
}
