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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class ABGameWorldBackUp : ABSingleton<ABGameWorld>
{

	static int _levelTimesTried;

	private bool _levelCleared;

	private List<ABPig> _pigs;
	private List<ABBird> _birds;
	private List<ABParticle> _birdTrajectory;


	private ABBird _lastThrownBird;
	private Transform _blocksTransform;
	private Transform _birdsTransform;
	private Transform _plaftformsTransform;
	private Transform _slingshotBaseTransform;

	private LevelLoader _levelLoader;

	private GameObject _slingshot;
	public GameObject Slingshot() { return _slingshot; }

	private GameObject _levelFailedBanner;
	public bool LevelFailed() { return _levelFailedBanner.activeSelf; }

	private GameObject _levelClearedBanner;
	public bool LevelCleared() { return _levelClearedBanner.activeSelf; }

	private int _pigsAtStart;
	public int PigsAtStart { get { return _pigsAtStart; } }

	private int _birdsAtStart;
	public int BirdsAtStart { get { return _birdsAtStart; } }

	private int _blocksAtStart;
	public int BlocksAtStart { get { return _blocksAtStart; } }

	public ABGameplayCamera GameplayCam { get; set; }
	public float LevelWidth { get; set; }
	public float LevelHeight { get; set; }

	// Game world properties
	public bool _isSimulation;
	public int _timesToGiveUp;
	public float _timeToResetLevel = 1f;
	public int _birdsAmounInARow = 5;

	public AudioClip[] _clips;



	/// /// For Subset
	public static double minicircle = 1;
	public static double maxcircle = 2;
	public Vector2 blockPosition;
	public float levelMaxSpeed = 0f;
	public static float platformStartPointX = 999f;
	public static float platformStartPointY;
	public ABLevel CurrentLevel;
	public ABLevel SymmetricalLevel;
	public LevelLoader levelLoader;
	public static bool generateLevel = false;
	public static int verticalBulletPosition = 0;
	public static int verticalTimes;
	public static int horizontalBulletPosition = 0;
	public static int horizontalTimes = 10;
	public static bool alreadyDropHorizontal = false;
	public static bool alreadyDropVertical = false;
	public static int blockId;
	public static bool horizontalEvaluationStart = false;


	// For Generate
	public static bool isGenerateSubset = false;
	public static bool isGenerateGround = false;
	public static bool isSaveLevel = false;
	public static bool isGenerateFirstGround = false;
	public static bool isGeneratesecondGround = false;
	public static List<Vector2> positionSubsets;
	public static int tempPosition;
	public static List<int> levelSubsets;
	public static int loopLimit = 0;
	public static int SubsetSimulationNumber; // 0 is Horizontal,1 is Vertical.
	public static List<int> groundIndex;
	public static int subsetCount = 0;
	public static int randomLevel;
	//	public static int countTNT;
	public static int verticalBulletPositionTest = 0;
	public static int horizontalBulletPositionTest = 0;
	public static int countNotExplode = 0;
	public static bool isShootTriggerPoint = false;
	public static bool isShootHorizontalTriggerPoint = false;
	public static bool isShootVerticalTriggerPoint = false;
	public static int maxGroundSubset = 0;
	public static Vector2 rangeTriggerPoint;
	public static bool alreadyDrop = false;
	public static List<Vector2> positions; //subset position

	void Awake()
	{

		_blocksTransform = GameObject.Find("Blocks").transform;
		_birdsTransform = GameObject.Find("Birds").transform;
		_plaftformsTransform = GameObject.Find("Platforms").transform;

		_levelFailedBanner = GameObject.Find("LevelFailedBanner").gameObject;
		_levelFailedBanner.gameObject.SetActive(false);

		_levelClearedBanner = GameObject.Find("LevelClearedBanner").gameObject;
		_levelClearedBanner.gameObject.SetActive(false);
		GameplayCam = GameObject.Find("Camera").GetComponent<ABGameplayCamera>();
	}

	// Use this for initialization
	void Start()
	{
		_pigs = new List<ABPig>();
		_birds = new List<ABBird>();
		_birdTrajectory = new List<ABParticle>();
		_levelLoader = new LevelLoader();
		_levelCleared = false;
		CurrentLevel = LevelList.Instance.GetCurrentLevel();
		levelLoader = new LevelLoader();

		positions = new List<Vector2>();
		positionSubsets = new List<Vector2>();
		levelSubsets = new List<int>();
		groundIndex = new List<int>();


		if (!_isSimulation)
		{
			GetComponent<AudioSource>().PlayOneShot(_clips[0]);
			GetComponent<AudioSource>().PlayOneShot(_clips[1]);
		}

		// If there are objects in the scene, use them to play
		if (_blocksTransform.childCount > 0 || _birdsTransform.childCount > 0)
		{

			foreach (Transform bird in _birdsTransform)
				AddBird(bird.GetComponent<ABBird>());

			foreach (Transform block in _blocksTransform)
			{

				ABPig pig = block.GetComponent<ABPig>();
				if (pig != null)
					_pigs.Add(pig);
			}

		}
		else
		{
			ABLevel currentLevel = LevelList.Instance.GetCurrentLevel();


			//ABLG!!
			//Randomize first subset position
			if (LevelSimulator.generateLevel)
				LevelSimulator.ChangeSubsetPosition(currentLevel, UnityEngine.Random.Range(-2, 2), UnityEngine.Random.Range(-3, 2));
			//


			if (currentLevel != null)
			{
				DecodeLevel(currentLevel);

				//ABLG!!
				//generateLevel
				if (LevelSimulator.generateLevel)
					GenerateLevel ();
				//

				AdaptCameraWidthToLevel();
				_levelTimesTried = 0;
				_slingshotBaseTransform = GameObject.Find("slingshot_base").transform;
			}
		}

		if (_isSimulation)
		{
			Time.timeScale = 100;
		}
	}

	public void initx()
	{
		ClearWorld();

		if (_levelFailedBanner.activeSelf)
			_levelFailedBanner.SetActive(false);

		if (_levelClearedBanner.activeSelf)
			_levelClearedBanner.SetActive(false);

		GameplayCam = GameObject.Find("Camera").GetComponent<ABGameplayCamera>();

		_pigs = new List<ABPig>();
		_birds = new List<ABBird>();
		_birdTrajectory = new List<ABParticle>();
		_levelCleared = false;

		if (!_isSimulation)
		{

			GetComponent<AudioSource>().PlayOneShot(_clips[0]);
			GetComponent<AudioSource>().PlayOneShot(_clips[1]);
		}

		ABLevel level = LevelList.Instance.GetCurrentLevel();
		DecodeLevel(level);

		_slingshotBaseTransform = GameObject.Find("slingshot_base").transform;
		_blocksTransform = GameObject.Find("Blocks").transform;
		_birdsTransform = GameObject.Find("Birds").transform;
		_plaftformsTransform = GameObject.Find("Platforms").transform;

		HUD.Instance.gameObject.SetActive(true);
		//if (LevelSimulator.generateLevel == false)
		//{
		//    CurrentLevel.usefulLevel = false;
		//}
	}

	public void DecodeLevel(ABLevel currentLevel)
	{

		ClearWorld();

		LevelHeight = ABConstants.LEVEL_ORIGINAL_SIZE.y;
		LevelWidth = (float)currentLevel.width * ABConstants.LEVEL_ORIGINAL_SIZE.x;

		Vector3 cameraPos = GameplayCam.transform.position;
		cameraPos.x = currentLevel.camera.x;
		cameraPos.y = currentLevel.camera.y;
		GameplayCam.transform.position = cameraPos;

		GameplayCam._minWidth = currentLevel.camera.minWidth;
		GameplayCam._maxWidth = currentLevel.camera.maxWidth;

		Vector3 landscapePos = ABWorldAssets.LANDSCAPE.transform.position;
		//		Vector3 backgroundPos = ABWorldAssets.BACKGROUND.transform.position;

		if (currentLevel.width > 1)
		{

			landscapePos.x -= LevelWidth / 4f;
			//			backgroundPos.x -= LevelWidth/ 4f;
		}

		for (int i = 0; i < currentLevel.width; i++)
		{

			GameObject landscape = (GameObject)Instantiate(ABWorldAssets.LANDSCAPE, landscapePos, Quaternion.identity);
			landscape.transform.parent = transform;

			float screenRate = currentLevel.camera.maxWidth / LevelHeight;
			if (screenRate > 2f)
			{

				for (int j = 0; j < (int)screenRate; j++)
				{

					Vector3 deltaPos = Vector3.down * (LevelHeight / 1.5f + (j * 2f));
					Instantiate(ABWorldAssets.GROUND_EXTENSION, landscapePos + deltaPos, Quaternion.identity);
				}
			}

			landscapePos.x += ABConstants.LEVEL_ORIGINAL_SIZE.x - 0.01f;
			//
			//			GameObject background = (GameObject)Instantiate(ABWorldAssets.BACKGROUND, backgroundPos, Quaternion.identity);
			//			background.transform.parent = GameplayCam.transform;
			//			backgroundPos.x += ABConstants.LEVEL_ORIGINAL_SIZE.x - 0.01f;
		}

		Vector2 slingshotPos = new Vector2(currentLevel.slingshot.x, currentLevel.slingshot.y);
		_slingshot = (GameObject)Instantiate(ABWorldAssets.SLINGSHOT, slingshotPos, Quaternion.identity);
		_slingshot.name = "Slingshot";
		_slingshot.transform.parent = transform;

		foreach (BirdData gameObj in currentLevel.birds)
		{

			AddBird(ABWorldAssets.BIRDS[gameObj.type], ABWorldAssets.BIRDS[gameObj.type].transform.rotation);
		}

		foreach (OBjData gameObj in currentLevel.pigs)
		{

			Vector2 pos = new Vector2(gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
			AddPig(ABWorldAssets.PIGS[gameObj.type], pos, rotation);
		}

		foreach (BlockData gameObj in currentLevel.blocks)
		{

			Vector2 pos = new Vector2(gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);

			GameObject block = AddBlock(ABWorldAssets.BLOCKS[gameObj.type], pos, rotation);

			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), gameObj.material);
			block.GetComponent<ABBlock>().SetMaterial(material);
		}

		foreach (PlatData gameObj in currentLevel.platforms)
		{
			if ((_isSimulation) && platformStartPointX > gameObj.x)
			{
				platformStartPointX = gameObj.x;
				platformStartPointY = gameObj.y;
			}
			Vector2 pos = new Vector2(gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);

			AddPlatform(ABWorldAssets.PLATFORM, pos, rotation, gameObj.scaleX, gameObj.scaleY);


			//Vector2 pos2 = new Vector2(gameObj.x+1, gameObj.y+2);
			//AddPlatform(ABWorldAssets.PLATFORM, pos2, rotation, gameObj.scaleX, gameObj.scaleY);

		}

		foreach (OBjData gameObj in currentLevel.tnts)
		{

			Vector2 pos = new Vector2(gameObj.x, gameObj.y);
			Quaternion rotation = Quaternion.Euler(0, 0, gameObj.rotation);
			GameObject addTNT = AddBlock(ABWorldAssets.TNT, pos, rotation);
			addTNT.tag = "TNT";
		}

		StartWorld();
	}

	//PreLevelCheck control the pre-check of subset(each level will be checked 1 time)
	bool PreLevelCheck = false;
	void Update()
	{

		// Check if birds was trown, if it died and swap them when needed

		//if(Input.GetKey(KeyCode.Space))
		//{
		//}

		//Evaluate Subsets
		if (LevelSimulator.generateLevel) { // is generate level
			if (!isShootTriggerPoint) // shoot trigger point
			{
				print("isShootTriggerPoint: ");
				if (!isShootHorizontalTriggerPoint)
				{
					ShootHorizontalTriggerPoint();
				}
				else 
					if (!isShootVerticalTriggerPoint)
					{
						ShootVerticalTriggerPoint();
					}
				RecordEveryNFrames(10);
			} else {
				//print("else isShootTriggerPoint: ");

				if (!isGenerateSubset)
				{ // isn't generate subset
					print("Will Generate");
					initx();
					randomLevel = UnityEngine.Random.Range(0, LevelList.Instance.GetAllLevel().Length);
					//Debug.Log("randomLevel " + randomLevel);

					for (int i = 0; i < positionSubsets.Count; i++)
					{ // generate old subset
						GenerateSubset(positionSubsets[i], levelSubsets[i]);
					}
					tempPosition = UnityEngine.Random.Range(positions.Count / 2, positions.Count);
					print("GENERATE X: " + positions[tempPosition].x);
					GenerateSubset(positions[tempPosition], randomLevel); // generate new subset
					//					CurrentLevel.countTNT++;
					AdaptCameraWidthToLevel();
					isGenerateSubset = true;
					isShootTriggerPoint = true;
				}

				if (isGenerateSubset)
				{ // Generate subset
					loopLimit++;
					//print("Loop: "+loopLimit);
					if (subsetCount > 1)
					{
						if (loopLimit == 250){
							print("subsetCount " + subsetCount);
							print("Next Level");
							SavegenerateLevel();
							isShootTriggerPoint = false;
							loopLimit = 0;
						}
					}
					else
					{ // count subset isn't 5
						//if (loopLimit == 100) {
						//    print("TEST: " + SubsetSimulationNumber);
						//    currentLevel.isTestTriggerPoint = true;
						//    if (SubsetSimulationNumber == 0) // Simulate again
						//    {
						//        TestHorizontalTriggerPoint();
						//    }
						//    if (SubsetSimulationNumber == 1)
						//    {
						//        TestVerticalTriggerPoint();
						//    }
						//    print("FINISH TEST");
						//}
						//if (currentLevel.isTNTExplode)
						if (loopLimit == 100)
						{ // TNT explode
							print("TNT Explode ");
							positionSubsets.Add(positions[tempPosition]);
							levelSubsets.Add(randomLevel);

							subsetCount++;
							positions.Clear();
							//							CurrentLevel.isTNTExplode = false;
							isShootHorizontalTriggerPoint = false;
							isShootVerticalTriggerPoint = false;
							isShootTriggerPoint = false;
							//							CurrentLevel.isTestTriggerPoint = false;
							CurrentLevel.usefulLevel = false;
							isGenerateSubset = false;
							loopLimit = 0;
						}
						//else if (loopLimit == 500 && !currentLevel.isTNTExplode)
						//{ // TNT not explode 
						//    print(loopLimit + " Not Explode");
						//    ABTNT.ResetCountExplode();
						//    currentLevel.countTNT--;

						//    if (countNotExplode > 5)
						//    {
						//        countNotExplode = 0;
						//        isShootTriggerPoint = false;
						//        NextLevel();
						//    }
						//    if (isGenerateFirstGround)
						//    {
						//        for (int i = 0; i < positions.Count; i++)
						//        {
						//            if (positions[i].y == -3.8f)
						//            {
						//                print("-3.888888ffffff");
						//                if (i == 0)
						//                {
						//                    print("ONE");
						//                    isGenerateFirstGround = false;
						//                    maxGroundSubset--;
						//                }
						//                else if (i == 1)
						//                {
						//                    print("TWO");
						//                    isGeneratesecondGround = false;
						//                    maxGroundSubset--;
						//                }
						//            }
						//        }
						//        isGenerateSubset = false;
						//        loopLimit = 0;
						//        countNotExplode++;
						//        positions.Clear();
						//        isShootTriggerPoint = false;
						//        currentLevel.isTestTriggerPoint = false;
						//        CurrentLevel.usefulLevel = false;
						//    }
						//}

					}
					//if (isSaveLevel)
					//{ // Next level
					//    isShootTriggerPoint = false;
					//    isSaveLevel = false;
					//    print("Next Level");
					//    SavegenerateLevel();
					//}
				}
			}

		}




		//Evaluate Subsets
		else
		{
			ManageBirds();
			if (!PreLevelCheck)
			{
				LevelShaking();
				if (IsLevelStable())
				{
					if (CurrentLevel.levelShaking)
						NextLevel();
					PreLevelCheck = true;
					CurrentLevel = levelLoader.EncodeLevel();
					SymmetricalLevel = levelLoader.EncodeSymmetricalLevel();
				}   

			}
			else
			{
				if (!horizontalEvaluationStart)
				{
					SubsetSimulationHorizontal();
				}
				else
				{
					SubsetSimulationVertical();
				}
				//check useful levels
				CheckUseful(_blocksTransform);
			}
		}
	}



	public bool IsObjectOutOfWorld(Transform abGameObject, Collider2D abCollider)
	{

		Vector2 halfSize = abCollider.bounds.size / 2f;

		if (abGameObject.position.x - halfSize.x > LevelWidth / 2f ||
			abGameObject.position.x + halfSize.x < -LevelWidth / 2f)

			return true;

		return false;
	}

	void ManageBirds()
	{

		if (_birds.Count == 0)
			return;

		// Move next bird to the slingshot
		if (_birds[0].JumpToSlingshot)
			_birds[0].SetBirdOnSlingshot();

		//		int birdsLayer = LayerMask.NameToLayer("Birds");
		//		int blocksLayer = LayerMask.NameToLayer("Blocks");
		//		if(_birds[0].IsFlying || _birds[0].IsDying)
		//			
		//			Physics2D.IgnoreLayerCollision(birdsLayer, blocksLayer, false);
		//		else 
		//			Physics2D.IgnoreLayerCollision(birdsLayer, blocksLayer, true);
	}

	public ABBird GetCurrentBird()
	{

		if (_birds.Count > 0)
			return _birds[0];

		return null;
	}

	public void NextLevel()
	{
		if (!CurrentLevel.usefulLevel)
			//		if (!CurrentLevel.usefulLevel||CurrentLevel.triggerX==0)
		{
			//            StreamWriter recordLevel = new StreamWriter(System.Environment.CurrentDirectory + "/Assets/StreamingAssets/Levels/levelcheck.txt", true);
			//            recordLevel.WriteLine((LevelList.Instance.CurrentIndex).ToString());
			//            recordLevel.Close();
		}
		else
		{
			//make Symmetrical subset of the current one, this function will be called in nextLevel() 
			levelLoader.SaveLevel(CurrentLevel, false);
			levelLoader.SaveLevel(SymmetricalLevel, true);
			//save the current level with adding TriggerPoint
			//LevelSave.SaveLevelOnScene();
		}

		horizontalBulletPosition = 0;
		verticalBulletPosition = 0;
		platformStartPointX = 999f;

		if (LevelList.Instance.NextLevel() == null) {
			if (!LevelSimulator.generateLevel)
				LevelSimulator.generateLevel = true;
			else {
				//				LevelSimulator.generateLevel = false;
				//				if (ABMenu.parameters.Count < 3)
				//					ABMenu.finished = true;
			}

			ABSceneManager.Instance.LoadScene("MainMenu");
		}
		else
		{

			ABSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().name);

		}

	}

	public void ResetLevel()
	{
		if (_levelFailedBanner.activeSelf)
			_levelTimesTried++;
		ABSceneManager.Instance.LoadScene(SceneManager.GetActiveScene().name);
	}

	public void AddTrajectoryParticle(ABParticle trajectoryParticle)
	{

		_birdTrajectory.Add(trajectoryParticle);
	}

	public void RemoveLastTrajectoryParticle()
	{

		foreach (ABParticle part in _birdTrajectory)
			part.Kill();
	}

	public void AddBird(ABBird readyBird)
	{

		if (_birds.Count == 0)
			readyBird.GetComponent<Rigidbody2D>().gravityScale = 0f;

		if (readyBird != null)
			_birds.Add(readyBird);
	}

	public GameObject AddBird(GameObject original, Quaternion rotation)
	{

		Vector3 birdsPos = _slingshot.transform.position - ABConstants.SLING_SELECT_POS;

		if (_birds.Count >= 1)
		{

			birdsPos.y = _slingshot.transform.position.y;

			for (int i = 0; i < _birds.Count; i++)
			{

				if ((i + 1) % _birdsAmounInARow == 0)
				{

					float coin = (UnityEngine.Random.value < 0.5f ? 1f : -1);
					birdsPos.x = _slingshot.transform.position.x + (UnityEngine.Random.value * 0.5f * coin);
				}

				birdsPos.x -= ABWorldAssets.BIRDS[original.name].GetComponent<SpriteRenderer>().bounds.size.x * 1.75f;
			}
		}

		GameObject newGameObject = (GameObject)Instantiate(original, birdsPos, rotation);
		Vector3 scale = newGameObject.transform.localScale;
		scale.x = original.transform.localScale.x;
		scale.y = original.transform.localScale.y;
		newGameObject.transform.localScale = scale;

		newGameObject.transform.parent = _birdsTransform;
		newGameObject.name = "bird_" + _birds.Count;

		ABBird bird = newGameObject.GetComponent<ABBird>();
		bird.SendMessage("InitSpecialPower", SendMessageOptions.DontRequireReceiver);

		if (_birds.Count == 0)
			bird.GetComponent<Rigidbody2D>().gravityScale = 0f;

		if (bird != null)
			_birds.Add(bird);

		return newGameObject;
	}

	public GameObject AddPig(GameObject original, Vector3 position, Quaternion rotation, float scale = 1f)
	{

		GameObject newGameObject = AddBlock(original, position, rotation, scale);

		ABPig pig = newGameObject.GetComponent<ABPig>();
		if (pig != null)
			_pigs.Add(pig);

		return newGameObject;
	}

	public GameObject AddPlatform(GameObject original, Vector3 position, Quaternion rotation, float scaleX = 1f, float scaleY = 1f)
	{
		//Debug.Log("add Platform");
		GameObject platform = AddBlock(original, position, rotation, scaleX, scaleY);
		platform.transform.parent = _plaftformsTransform;

		return platform;
	}

	public GameObject AddBlock(GameObject original, Vector3 position, Quaternion rotation, float scaleX = 1f, float scaleY = 1f)
	{

		GameObject newGameObject = (GameObject)Instantiate(original, position, rotation);
		newGameObject.transform.parent = _blocksTransform;

		Vector3 newScale = newGameObject.transform.localScale;
		newScale.x = scaleX;
		newScale.y = scaleY;
		newGameObject.transform.localScale = newScale;

		return newGameObject;
	}

	private void ShowLevelFailedBanner()
	{

		if (_levelCleared)
			return;

		if (!IsLevelStable())
		{

			Invoke("ShowLevelFailedBanner", 1f);
		}
		else
		{

			// Player lost the game
			HUD.Instance.gameObject.SetActive(false);

			if (_levelTimesTried < _timesToGiveUp - 1)
			{

				_levelFailedBanner.SetActive(true);
			}
			else
			{

				_levelClearedBanner.SetActive(true);
				_levelClearedBanner.GetComponentInChildren<Text>().text = "Level Failed!";
			}
		}
	}

	private void ShowLevelClearedBanner()
	{
		if (!IsLevelStable())
		{

			Invoke("ShowLevelClearedBanner", 1f);
		}
		else
		{

			// Player won the game
			HUD.Instance.gameObject.SetActive(false);

			_levelClearedBanner.SetActive(true);
			_levelClearedBanner.GetComponentInChildren<Text>().text = "Level Cleared!";
		}
	}

	public void KillPig(ABPig pig)
	{

		_pigs.Remove(pig);

		if (_pigs.Count == 0)
		{

			// Check if player won the game
			if (!_isSimulation)
			{

				_levelCleared = true;
				Invoke("ShowLevelClearedBanner", _timeToResetLevel);
			}

			return;
		}
	}

	public void KillBird(ABBird bird)
	{

		if (!_birds.Contains(bird))
			return;

		_birds.Remove(bird);

		if (_birds.Count == 0)
		{

			// Check if player lost the game
			if (!_isSimulation)
				Invoke("ShowLevelFailedBanner", _timeToResetLevel);

			return;
		}

		_birds[0].GetComponent<Rigidbody2D>().gravityScale = 0f;
		_birds[0].JumpToSlingshot = true;
	}

	public int GetPigsAvailableAmount()
	{

		return _pigs.Count;
	}

	public int GetBirdsAvailableAmount()
	{

		return _birds.Count;
	}

	public int GetBlocksAvailableAmount()
	{

		int blocksAmount = 0;

		foreach (Transform b in _blocksTransform)
		{

			if (b.GetComponent<ABPig>() == null)

				for (int i = 0; i < b.GetComponentsInChildren<Rigidbody2D>().Length; i++)
					blocksAmount++;
		}

		return blocksAmount;
	}

	public bool IsLevelStable()
	{

		return GetLevelStability() == 0f;
	}

	public float GetLevelStability()
	{

		float totalVelocity = 0f;

		foreach (Transform b in _blocksTransform)
		{

			Rigidbody2D[] bodies = b.GetComponentsInChildren<Rigidbody2D>();

			foreach (Rigidbody2D body in bodies)
			{

				if (!IsObjectOutOfWorld(body.transform, body.GetComponent<Collider2D>()))
					totalVelocity += body.velocity.magnitude;
			}
		}

		return totalVelocity;
	}

	public List<GameObject> ObjectsInScene()
	{

		List<GameObject> objsInScene = new List<GameObject>();

		foreach (Transform b in _blocksTransform)
			objsInScene.Add(b.gameObject.transform.gameObject);
		return objsInScene;
	}

	public Vector3 DragDistance()
	{

		Vector3 selectPos = (_slingshot.transform.position - ABConstants.SLING_SELECT_POS);
		return _slingshotBaseTransform.transform.position - selectPos;
	}

	public void SetSlingshotBaseActive(bool isActive)
	{

		_slingshotBaseTransform.gameObject.SetActive(isActive);
	}

	public void ChangeSlingshotBasePosition(Vector3 position)
	{

		_slingshotBaseTransform.transform.position = position;
	}

	public void ChangeSlingshotBaseRotation(Quaternion rotation)
	{

		_slingshotBaseTransform.transform.rotation = rotation;
	}

	public bool IsSlingshotBaseActive()
	{

		return _slingshotBaseTransform.gameObject.activeSelf;
	}

	public Vector3 GetSlingshotBasePosition()
	{

		return _slingshotBaseTransform.transform.position;
	}

	public void StartWorld()
	{

		_pigsAtStart = GetPigsAvailableAmount();
		_birdsAtStart = GetBirdsAvailableAmount();
		_blocksAtStart = GetBlocksAvailableAmount();

	}

	public void ClearWorld()
	{

		foreach (Transform b in _blocksTransform)
			Destroy(b.gameObject);

		_pigs.Clear();

		foreach (Transform b in _birdsTransform)
			Destroy(b.gameObject);

		foreach (Transform b in _plaftformsTransform)
			Destroy(b.gameObject);



		_birds.Clear();
	}

	private void AdaptCameraWidthToLevel()
	{

		Collider2D[] bodies = _blocksTransform.GetComponentsInChildren<Collider2D>();

		if (bodies.Length == 0)
			return;

		// Adapt the camera to show all the blocks		
		float levelLeftBound = -LevelWidth / 2f;
		float groundSurfacePos = LevelHeight / 2f;

		float minPosX = Mathf.Infinity;
		float maxPosX = -Mathf.Infinity;
		float maxPosY = -Mathf.Infinity;

		// Get position of first non-empty stack
		for (int i = 0; i < bodies.Length; i++)
		{
			float minPosXCandidate = bodies[i].transform.position.x - bodies[i].bounds.size.x / 2f;
			if (minPosXCandidate < minPosX)
				minPosX = minPosXCandidate;

			float maxPosXCandidate = bodies[i].transform.position.x + bodies[i].bounds.size.x / 2f;
			if (maxPosXCandidate > maxPosX)
				maxPosX = maxPosXCandidate;

			float maxPosYCandidate = bodies[i].transform.position.y + bodies[i].bounds.size.y / 2f;
			if (maxPosYCandidate > maxPosY)
				maxPosY = maxPosYCandidate;
		}

		float cameraWidth = Mathf.Abs(minPosX - levelLeftBound) +
			Mathf.Max(Mathf.Abs(maxPosX - minPosX), Mathf.Abs(maxPosY - groundSurfacePos)) + 0.5f;

		GameplayCam.SetCameraWidth(cameraWidth);
	}









	/// <summary>
	/// ABLG!!
	/// Functions for evaluating Subsets
	/// </summary>

	//start Horizontal test
	public void SubsetSimulationHorizontal()
	{
		if (!_isSimulation || !IsLevelStable())
			return;

		if (!alreadyDropHorizontal)
		{
			float y = horizontalBulletPosition * 0.31f + ABGameWorld.platformStartPointY + 0.5f;
			float x = ABGameWorld.platformStartPointX - 0.62f;

			Vector2 pos = new Vector2(x, y);
			//            Vector2 force = new Vector2(2, 0);
			Quaternion rotation = Quaternion.Euler(0, 0, 0);

			GameObject block = AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], pos, rotation);
			block.tag = "test";
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
			block.GetComponent<ABBlock>().SetMaterial(material);
			block.GetComponent<Rigidbody2D> ().velocity = new Vector2 (4f, 0);
			//            block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
			horizontalBulletPosition++;


			if (horizontalBulletPosition > horizontalTimes)
			{
				horizontalBulletPosition = 0;
				horizontalEvaluationStart = true;
				initx();
				return;
			}
			alreadyDropHorizontal = true;
		}
		else
		{
			initx();
			alreadyDropHorizontal = false;
		}

	}

	//start Vertical test
	public void SubsetSimulationVertical()
	{
		if (!_isSimulation || !IsLevelStable())
			return;

		if (!alreadyDropVertical)
		{
			verticalTimes = CurrentLevel.platforms.Count + 1;
			float x = verticalBulletPosition * 0.62f + ABGameWorld.platformStartPointX - 0.32f;
			float y = ABGameWorld.platformStartPointY + 5f;
			Vector2 pos = new Vector2(x, y);
			//            Vector2 force = new Vector2(0, -2f);
			Quaternion rotation = Quaternion.Euler(0, 0, 0);
			GameObject block = AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], pos, rotation);
			block.tag = "test";
			//            block.GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);
			block.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -4f);
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
			block.GetComponent<ABBlock>().SetMaterial(material);
			verticalBulletPosition++;
			if (verticalBulletPosition > verticalTimes)
			{
				verticalBulletPosition = 0;
				horizontalEvaluationStart = false;
				print ("Useless Subset");
				NextLevel();
				return;
			}

			alreadyDropVertical = true;
		}
		else
		{
			initx();
			alreadyDropVertical = false;
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
			//			if (b.tag == "test") {
			//				collisionPoint = b.GetComponent<Collision2D> ().contacts [0].point;
			//				CurrentLevel.triggerX = collisionPoint.x;
			//				CurrentLevel.triggerY = collisionPoint.y;
			//				print ("asdfasdf");
			//			}
			if ((b.tag != "test") && b.position.y < ABGameWorld.platformStartPointY - 0.5f)
			{

				CurrentLevel.usefulLevel = true;
				NextLevel();
			}
		}
	}

	public void LevelShaking()
	{
		if (_blocksTransform.childCount == 0)
			return;
		foreach (Transform block in _blocksTransform)
		{
			if (block.GetComponent<Rigidbody2D>().velocity.magnitude > 0.3f)
			{
				CurrentLevel.levelShaking = true;
				levelLoader.SaveLevel(CurrentLevel, false);
				CurrentLevel.usefulLevel = false;
			}

		}
	}








	/// <summary>
	/// ABLG!!
	/// Functions for Generating Levels
	/// </summary>


	public void GenerateLevel () {
		/// Generate Level
		if (LevelSimulator.generateLevel)
		{
			int round = UnityEngine.Random.Range(2, 3);
			for (int i = 0; i < round; i++)
			{
				//Random subset for generate level
				int selectedLevel = UnityEngine.Random.Range(0, LevelList.Instance.GetAllLevel().Length/2);
				ABLevel nextLevelSubset = LevelList.Instance.GetLevel(selectedLevel);
				LevelSimulator.ChangeSubsetPosition(nextLevelSubset, UnityEngine.Random.Range(4f, 6f), UnityEngine.Random.Range(-3f, 2f));
				LevelSimulator.GenerateSubset(nextLevelSubset, nextLevelSubset.triggerX, nextLevelSubset.triggerY);
			}
			//Save generate level on scene to xml
			_levelLoader.SaveLevelOnScene();
		}
	}

	public void ShootHorizontalTriggerPoint() {
		if (!_isSimulation || !IsLevelStable()) {
			return;
		}

		if (!alreadyDropHorizontal)
		{
			print("DROP H");
			float y = (CurrentLevel.triggerY + 2f); // + currentLevel.triggerY
			float x = (CurrentLevel.triggerY - 2f) ; //+ currentLevel.triggerX
			//			Debug.Log("X "+CurrentLevel.platformStartPoint.x+",Y "+CurrentLevel.platformStartPoint.y);
			GameObject block = AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], new Vector2(x, y), Quaternion.Euler(0, 0, 0));
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
			block.GetComponent<ABBlock>().SetMaterial(material);
			block.GetComponent<Rigidbody2D>().AddForce(new Vector2(0.7f, 0.7f), ForceMode2D.Impulse);
			block.tag = "test";
			SubsetSimulationNumber = 0;
			horizontalBulletPosition++;
			//isShootHorizontalTriggerPoint = true;
			if (horizontalBulletPosition > horizontalTimes)
			{
				print("----- H");
				//ABTNT.ResetCountExplode();
				//isShootHorizontalTriggerPoint = true;
				//horizontalBulletPosition = 0;
				//HorizontalStart = true;
				//return;
				//isSaveLevel = true;
				//SavegenerateLevel();
			}
			alreadyDropHorizontal = true;
		}
		else
		{
			print("initx H");
			initx();
			for (int i = 0; i < positionSubsets.Count; i++)
			{
				GenerateSubset(positionSubsets[i], levelSubsets[i]);
			}
			alreadyDropHorizontal = false;
		}
	}

	public void ShootVerticalTriggerPoint() {
		if (!_isSimulation || !IsLevelStable()) {
			print("Return V");
			return;
		}

		if (!alreadyDrop)
		{
			print("DROP V");
			float x = CurrentLevel.triggerX + 2f;
			float y = CurrentLevel.triggerY + 4f;
			GameObject block = AddBlock(ABWorldAssets.BLOCKS["CircleSmall"], new Vector2(x, y), Quaternion.Euler(0, 0, 0));
			MATERIALS material = (MATERIALS)System.Enum.Parse(typeof(MATERIALS), "stone");
			block.GetComponent<ABBlock>().SetMaterial(material);
			block.GetComponent<Rigidbody2D>().AddForce(new Vector2(0, -0.5f), ForceMode2D.Impulse);
			block.tag = "test";
			SubsetSimulationNumber = 1;
			verticalBulletPosition++;
			//if (verticalBulletPosition > verticalTimes)
			//{
			//    print("----- V");
			//    verticalBulletPosition = 0;
			//    HorizontalStart = false;
			//    isShootVerticalTriggerPoint = true;

			//    //isSaveLevel = true;
			//    //SavegenerateLevel();
			//    return;
			//}
			alreadyDrop = true;
		}
		else
		{
			print("initx V");

			initx();
			for (int i = 0; i < positionSubsets.Count; i++)
			{
				GenerateSubset(positionSubsets[i], levelSubsets[i]);
			}
			alreadyDrop = false;
		}


	}


	public void SavegenerateLevel() {
		if (subsetCount > 0)
		{
			//			_levelLoader.SaveLevelOnScene();
			print("Save");
		} else {
			print("Not Save");
		}
		subsetCount = 0;
		//isGenerateSubset = false;
		//isGenerateGround = false;
		//CurrentLevel.usefulLevel = false;
		NextLevel();
		//isSaveLevel = false;
		//alreadyDrop = false;
		//alreadyDropHorizontal = false;
	}

	public void GenerateSubset(Vector2 position,int level) {
		ABLevel generateSubset = LevelList.Instance.GetLevel(level);
		LevelSimulator.GenerateSubset(generateSubset, position.x, position.y);
		//		countTNT = generateSubset.tnts.Count;
	}

	public void RecordEveryNFrames(int repeatRate)
	{
		if (Time.frameCount % repeatRate != 0)
		{
			return;
		}
		if (true)
		{
			foreach (Transform trans in GameObject.Find("Blocks").transform)
			{
				var objVel = trans.GetComponent<Rigidbody2D>().velocity;
				var objPos = trans.position;
				if ((objPos.x < 14.0f) && (objPos.y < 5.0f) && (objPos.y > -1.0f) && trans.tag != "test" && (objPos.x < CurrentLevel.triggerX - 5.0f)) // Sky  && (objPos.x > subsetStartPointX + 5.0f) && (objVel.x > velocityLimitationX)
				{
					if (positionSubsets.Count == 0) // Generate first subset
					{
						//if (objPos.x > 0) {
						print("First Subset "+ objPos);
						positions.Add(objPos);
						isShootTriggerPoint = true;
						//}
					}
					else // Generate other subset
					{
						//print("-------");
						//foreach(Vector2 p in positionSubsets) {
						//    print("PX " + p.x);
						//}
						//print("_____");
						for (int i = 0; i < positionSubsets.Count; i++)
						{
							if (objPos.x + 3.5f < positionSubsets[i].x || objPos.x - 3.5f > positionSubsets[i].x) //ใหม่ น้อยกว่า เก่า หรือ ใหม่ มากกว่า เก่า  || 
							{
								//float valueOut1 = positionSubsets[i].x - 3.0f;
								//float valueOut2 = positionSubsets[i].x + 3.0f;
								//print(i+" out loop New Pos " + objPos.x + ", Old " +valueOut1 + ", "+valueOut2);

								if (i == positionSubsets.Count - 1) {
									positions.Add(objPos);
									//float valueIn1 = positionSubsets[i].x - 3.0f;
									//float valueIn2 = positionSubsets[i].x + 3.0f;
									//print(i+" in loop Pos " + objPos.x + ", Old " + valueIn1 + ", "+valueIn2);
								}
							} else {
								return;
							}
						}

					}
				}

				if (positionSubsets.Count > 0 && objPos.x > -1.0f && objPos.y < -2.2f && objPos.x < 14.0f && trans.tag != "test" && maxGroundSubset < 2) //Ground
				{
					for (int k = 0; k < positionSubsets.Count; k++) {
						if (!isGenerateFirstGround)
						{
							if (k == positionSubsets.Count - 1) {
								print("Generate First Ground " + objPos.x);
								positions.Add(new Vector2(objPos.x, -3.8f));
								isShootTriggerPoint = true;
								isGenerateFirstGround = true;
								maxGroundSubset++;
							}

						}
					}

					if (isGenerateFirstGround) {
						for (int i = 0; i < positionSubsets.Count; i++)
						{
							if (positionSubsets[i].y == -3.8f)
							{
								if (objPos.x + 3.5f < positionSubsets[i].x || objPos.x - 3.5f > positionSubsets[i].x)
								{
									print("Generate Ground " + objPos.x + " " + positionSubsets[i].x);
									positions.Add(new Vector2(objPos.x, -3.8f));
									isShootTriggerPoint = true;
									maxGroundSubset++;
									isGeneratesecondGround = true;
								} else {
									return;
								}
							}
						}
					}

				}
			}
		}
	}
}