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

public class ABMenu : MonoBehaviour {
	/// <summary>
	/// This Start() is activating GameWolrd Scene directly
	/// Always start from level 1.
	/// </summary>
	void Start() {
		if (UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name != "GameWorld" && !LevelSimulator.Generatelevel) {
			try {
				LevelList.Instance.SetLevel (1);
				LoadNextScene ("GameWorld", false, null);
			} catch {
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
}
