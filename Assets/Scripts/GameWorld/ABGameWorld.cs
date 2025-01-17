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

public class ObjTrack
{
	public GameObject obj { get; set;}
	public Vector2 pos { get; set;}

}

public class SubsetInfo
{
	public Vector2 triggerPoint { get; set; }

	public ABLevel lvl { get; set; }

	public int id { get; set; }

	public bool horizontal { get; set;}

	public Vector2 center { get; set;}

	public float movement { get; set;}
}

public class ABGameWorld : ABSingleton<ABGameWorld>
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
    public float LevelMaxSpeed = 0f;
    public static float platformStartPointX = 999f;
    public static float platformStartPointY;
    public ABLevel CurrentLevel;
    public ABLevel SymmetricalLevel;
    public static int VerticalBulletPosition = 0;
    public static int VerticalTimes;
    public static int HorizonalBulletPosition = 0;
    public static int HorizontalTimes = 10;
    public static bool AlreadyDropHorizontal = false;
    public static bool AlreadyDropVertical = false;
    public static int BlockId;
    public static bool HorizontalEvaluationStart = false;

	public bool initiate = true;
	public List<ObjTrack> trajectory;
	public List<SubsetInfo> subsetList= new List<SubsetInfo> ();

	private Dictionary<string,Vector2> blockSize = new Dictionary<string,Vector2> () {
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"RectFat",new Vector2(0.84f,0.84f)},
		{"RectFat",new Vector2(0.84f,0.84f)},
		{"SquareSmall",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
		{"SquareHole",new Vector2(0.84f,0.84f)},
	}


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
        _levelCleared = false;
        CurrentLevel = LevelList.Instance.GetCurrentLevel();
        _levelLoader = new LevelLoader();
		trajectory = new List<ObjTrack>();
//		subsetList = new Dictionary<int,SubsetInfo> ();



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
//            ABLevel currentLevel = LevelList.Instance.GetCurrentLevel();


			//ABLG!!
			//Randomize first subset position
			if (LevelSimulator.generateLevel) {
//				LevelSimulator.ChangeSubsetPosition (currentLevel, UnityEngine.Random.Range (-2f, 2f), UnityEngine.Random.Range (-1f, 2f));
				Vector2 temp = new Vector2(-4f,1f); 
				LevelSimulator.ChangeSubsetPosition (CurrentLevel, temp.x,temp.y);
				AddSubsetIntoList (0, CurrentLevel, temp);
			}
			//


			if (CurrentLevel != null)
            {
				DecodeLevel(CurrentLevel);
                AdaptCameraWidthToLevel();
                _levelTimesTried = 0;
                _slingshotBaseTransform = GameObject.Find("slingshot_base").transform;
            }
        }

        if (_isSimulation)
        {
            Time.timeScale = 2;
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
//        ManageBirds();
        //if(Input.GetKey(KeyCode.Space))
        //{
        //}

        //Evaluate Subsets

		if (!LevelSimulator.generateLevel) {
			if (!PreLevelCheck) {
				LevelShaking ();
				if (IsLevelStable ()) {
					if (CurrentLevel.levelShaking)
						NextLevel ();
					PreLevelCheck = true;
//					CurrentLevel = _levelLoader.EncodeLevel ();
					SymmetricalLevel = _levelLoader.EncodeSymmetricalLevel ();
				}   

			} else {
				if (!HorizontalEvaluationStart) {
					CurrentLevel.horizontal = true;
					SymmetricalLevel.horizontal = true;
					SubsetSimulationHorizontal ();	
				} else {	
					CurrentLevel.horizontal = false;
					SymmetricalLevel.horizontal = false;
					SubsetSimulationVertical ();
				}
				//check useful levels
				CheckUseful (_blocksTransform);
			}
		} else 
			//Generate Levels
		{

			CheckOverlap ();
			
			if (initiate) {
				trajectory = new List<ObjTrack> ();
				ShootInitiator ();

				AdaptCameraWidthToLevel ();
			} else if (!IsLevelStable ())
				RecordTrajectory ();
//			else (!initiate && IsLevelStable ()) {
			else {
//				DeleteStaticSubsetFromList ();
				SaveCurrentLevel ();
				int levelID = UnityEngine.Random.Range (0, LevelList.Instance.GetAllLevel ().Length);
				if (trajectory.Count == 0) {
					
					LevelList.Instance.CurrentIndex = UnityEngine.Random.Range (0, levelID);
					ABSceneManager.Instance.LoadScene (SceneManager.GetActiveScene ().name);
					subsetList= new List<SubsetInfo> ();
					print ("Finding different initial subset...");
					return;
				}

				Vector2 subsetPos = trajectory [UnityEngine.Random.Range (trajectory.Count/2, trajectory.Count)].pos;
				ABLevel lvl = LevelList.Instance.GetLevel (levelID);
				AddSubsetIntoList (levelID, lvl, subsetPos);
				initiate = true;
				initx ();
//				mapChanges.Add (trans.GetInstanceID (), new MapChanges {
//					lastPosition = trans.position,
//					distanceObjectMoved = 0f,
//					timeObjectMoved = 0f
//				});
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
			if (!HorizontalEvaluationStart) {
				print ("HORIZONTAL");
				CurrentLevel.horizontal = true;
				SymmetricalLevel.horizontal = true;
			} else {
				print ("VERTICAL");
				CurrentLevel.horizontal = false;
				SymmetricalLevel.horizontal = false;
			}
            //make Symmetrical subset of the current one, this function will be called in nextLevel() 
			_levelLoader.SaveLevel(CurrentLevel, false);
			_levelLoader.SaveLevel(SymmetricalLevel, true);
            //save the current level with adding TriggerPoint
            //LevelSave.SaveLevelOnScene();
        }
        HorizonalBulletPosition = 0;
        VerticalBulletPosition = 0;
        platformStartPointX = 999f;
		if (LevelList.Instance.NextLevel() == null) {
			if (!LevelSimulator.generateLevel)
				LevelSimulator.generateLevel = true;
			else {
				LevelSimulator.generateLevel = false;
				if (ABMenu.parameters.Count < 3)
					ABMenu.finished = true;
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

        if (!AlreadyDropHorizontal)
        {
            float y = HorizonalBulletPosition * 0.31f + ABGameWorld.platformStartPointY + 0.6f;
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
            HorizonalBulletPosition++;
            

            if (HorizonalBulletPosition > HorizontalTimes)
            {
                HorizonalBulletPosition = 0;
                HorizontalEvaluationStart = true;
                initx();
                return;
            }
            AlreadyDropHorizontal = true;
        }
        else
        {
            initx();
            AlreadyDropHorizontal = false;
        }

    }

    //start Vertical test
    public void SubsetSimulationVertical()
    {
        if (!_isSimulation || !IsLevelStable())
            return;

        if (!AlreadyDropVertical)
        {
            VerticalTimes = CurrentLevel.platforms.Count + 1;
            float x = VerticalBulletPosition * 0.62f + ABGameWorld.platformStartPointX - 0.32f;
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
            VerticalBulletPosition++;
            if (VerticalBulletPosition > VerticalTimes)
            {
                VerticalBulletPosition = 0;
                HorizontalEvaluationStart = false;
				print ("Useless Subset");
                NextLevel();
                return;
            }

            AlreadyDropVertical = true;
        }
        else
        {
            initx();
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
				_levelLoader.SaveLevel(CurrentLevel, false);
                CurrentLevel.usefulLevel = false;
            }

        }
    }








	/// <summary>
	/// ABLG!!
	/// Functions for Generating Levels
	/// </summary>


	public void GenerateSubset(Vector2 position,int tag,int level) {
		ABLevel generateSubset = LevelList.Instance.GetLevel(level);
		LevelSimulator.GenerateSubset(generateSubset, tag, position.x, position.y);
//		Debug.Log ("pos : " + position.ToString () + ", trigger : " + generateSubset.triggerX.ToString () + generateSubset.triggerY.ToString ());
		//		countTNT = generateSubset.tnts.Count;
	}

	public void ShootInitiator () {
//		if (CurrentLevel.triggerX == 0) {
//			return;
//		}

		for (int i = 1; i < subsetList.Count; i++)
		{ // generate old subset
			GenerateSubset(subsetList[i].triggerPoint,i, subsetList[i].id);
			print ("id : " + subsetList [i].id.ToString () + ", trigger : " + subsetList [i].triggerPoint.ToString ());
		}
		if (GetLevelStability ()>0.5f) {
			subsetList.RemoveAt (subsetList.Count - 1);
			initx ();
			print ("Subset is unstable!");
			return;
		}
		Destroy (GameObject.FindGameObjectWithTag ("test"));
		Vector2 shootPos;
		GameObject block;
		MATERIALS material = (MATERIALS)System.Enum.Parse (typeof(MATERIALS), "stone");
		if (CurrentLevel.horizontal) {
			shootPos = new Vector2 (CurrentLevel.triggerX - 0.4f, CurrentLevel.triggerY+0.1f);
			block = AddBlock (ABWorldAssets.BLOCKS ["CircleSmall"], shootPos, Quaternion.Euler (0, 0, 0));
			block.GetComponent<Rigidbody2D> ().velocity = new Vector2 (4f, 0);
		} else {
			shootPos = new Vector2 (CurrentLevel.triggerX, CurrentLevel.triggerY + 0.4f);
			block = AddBlock (ABWorldAssets.BLOCKS ["CircleSmall"], shootPos, Quaternion.Euler (0, 0, 0));
			block.GetComponent<Rigidbody2D> ().velocity = new Vector2 (0, -4f);
		}
		block.GetComponent<ABBlock>().SetMaterial(material);
		block.tag = "test";
		initiate = false;
//		block2.GetComponent<ABBlock>().SetMaterial(material);
//		block2.GetComponent<Rigidbody2D> ().velocity = new Vector2 (2f, -2f);
//		block2.tag = "test";
	}

	public void RecordTrajectory() {
//		foreach (ObjTrack track in trajectory) {
//			GameObject obj = track.obj;
//		}
		GetSubsetMovement();
		if (Time.renderedFrameCount % 100 != 0 && Time.timeScale == 1) 
			return;
//		print ("recordTrajectory");
		foreach (Transform trans in GameObject.Find("Blocks").transform) {
			if (trans.GetComponent<Rigidbody2D> ().velocity.magnitude > 2.5f && trans.tag !="test" && trans.GetComponent<Rigidbody2D> ().velocity.y <0.5f) {
				ObjTrack track = new ObjTrack {
					obj = trans.gameObject,
					pos = trans.position
				};
				if (IsPointInScreen(track.pos)) {
					trajectory.Add (track);
				}
				foreach (SubsetInfo info in subsetList) {
					if (Vector2.Distance(info.triggerPoint,trans.position)<2f) {
						trajectory.Remove (track);
						break;
					}
				}

			}
		}
	}

	public void DeleteSubset () {
		subsetList.RemoveAt (subsetList.Count - 1);
	}

	public void GetSubsetMovement()
	{
//		foreach(SubsetInfo subset in subsetList) {
		for (int i = 0; i<subsetList.Count-1;i++) {
			SubsetInfo subset = subsetList [i];
			List<Rigidbody2D> bodies = new List<Rigidbody2D>();
			foreach (Transform trans in GameObject.Find("Blocks").transform) {
				if (Vector2.Distance(trans.position,subset.center)<2.12f)
					bodies.Add(trans.GetComponent<Rigidbody2D> ());

			}
			foreach (Rigidbody2D body in bodies) {

				if (!IsObjectOutOfWorld (body.transform, body.GetComponent<Collider2D> ())) {
					subset.movement += body.velocity.magnitude;
//					if (i != 0 )
//						print ("subset.movement : " + subset.movement.ToString()+", tag : "+i.ToString());
				}
			}
		}

		return;
	}

	public void DeleteStaticSubsetFromList () {
		foreach (SubsetInfo subset in subsetList) {
			if (subset.movement < 1f) {
				subsetList.Remove (subset);
			}
		}
		
	}

	public void AddSubsetIntoList (int id, ABLevel lvl, Vector2 pos) {
		Vector2 temp = new Vector2 (0, 0);
		foreach (BlockData b in lvl.blocks) {
			temp += new Vector2 (b.x, b.y);
		}
		foreach (PlatData p in lvl.platforms) {
			temp += new Vector2 (p.x, p.y);
		}
		temp /= (lvl.platforms.Count + lvl.blocks.Count);
		subsetList.Add (new SubsetInfo {
			id = id,
			triggerPoint = pos,
			lvl = lvl,
			horizontal = lvl.horizontal,
			center = temp,
			movement = 0
		});
	}

	public bool IsPointInScreen (Vector2 vec) {
		if (vec.x > -7.15 && vec.x < 12 && vec.y < 7 && vec.y > -1.0) {
			return true;
		} else
			return false;
	}

	private void IsSubsetStable () {
//		for(int i =0;i<subsetList.Count-1;)
	}

	private void CheckOverlap () {
		foreach (Transform trans in GameObject.Find("Blocks").transform) {
			float temp = trans.GetComponent<Rigidbody2D> ().velocity.magnitude;
			if (temp > 20f) {
				subsetList.RemoveAt (subsetList.Count - 1);
				initx ();
				initiate = true;
				print ("Overlapped");
				return;
			}
		}
	}

	private void SaveCurrentLevel () {
		if (subsetList.Count > 1) {
			initx ();
			for (int i = 1; i < subsetList.Count; i++)
			{ // generate old subset
				GenerateSubset(subsetList[i].triggerPoint,i, subsetList[i].id);
//				print ("id : " + subsetList [i].id.ToString () + ", trigger : " + subsetList [i].triggerPoint.ToString ());
			}
			_levelLoader.SaveLevelOnScene ();
			print ("Level Saved!");
		}
	}

	public void AddPigIntoSubset (ABLevel level) {
		if (level.tnts.Count != 0) {
			//string[] types = new string[3]{"BasicBig","BasicMedium","BasicSmall"};
			//string type = types[UnityEngine.Random.Range(0,types.Length)];
			string type = "BasicSmall";
			int randomTNT = UnityEngine.Random.Range (0, level.tnts.Count); // find tnt
			OBjData tnt = level.tnts[randomTNT];
			level.pigs.Add(new OBjData (type, 0, tnt.x, tnt.y + 0.5f)); // add pig
		}
	}

	public void ChangePigIntoBlock(ABLevel level) {
		if (level.pigs.Count != 0) {
			string[] types = new string[2]{"CircleSmall","RectTiny"};
			string type = types[UnityEngine.Random.Range(0,types.Length)]; //random block type

			string[] materials = new string[3]{"wood","ice","stone"};
			string material = materials[UnityEngine.Random.Range(0,materials.Length)]; //random block material

			int randomPig = UnityEngine.Random.Range (0, level.pigs.Count); // find pig
			OBjData pig = level.pigs [randomPig]; //Get pig`

			level.blocks.Add(new BlockData(type, pig.rotation, pig.x, pig.y, material)); // add block
			level.pigs.RemoveAt (randomPig); // remove pig
		}
	}
}