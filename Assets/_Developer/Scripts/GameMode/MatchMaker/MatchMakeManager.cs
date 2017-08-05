using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakeManager : MonoBehaviour {

	public static MatchMakeManager Instance;

	#region CUSTOM - DATA TYPE

	[System.Serializable]
	public struct GridType
	{
		public Sprite gridSprite;
		public GameObject gridDestroyParticle;
	}

	[System.Serializable]
	public struct GridAttributes
	{
		public int gridType;

		[HideInInspector]
		public GameObject gridObject;
		[HideInInspector]
		public GameObject gridDestroyParticle;

		[HideInInspector]
		public bool isGridSolved;

		[HideInInspector]
		public bool isGridAppearAnimationRunning;
		[HideInInspector]
		public bool isGridDisappearAnimationRunning;

		[HideInInspector]
		public float nextRotationTime;
		[HideInInspector]
		public int rotationCounter;

	}

	#endregion

	#region PUBLIC VARIABLES

	public Camera defaultCamera;

	[Header("Grid Property:")]
	[Range(1,10)]
	public float distanceAmongEachGridArea;
	public Sprite defaultGridSprite;
	public GameObject gridPrefab;

	[Space]
	[Header("--------------")]
	[Tooltip("In MS (Milisecond)")]
	[Range(0.0f,1000.0f)]
	public float intervalOnEachRotation;
	[Range(0.0f,360.0f)]
	public float rotationAmount;
	[Range(0,36)]
	public int numberOfRotation;

	[Header("--------------")]
	public int baseRow;
	public int baseColumn;
	[Range(0,10)]
	public int highestDifficultyLevel;
	public int focusGridType;
	[Range(0.0f,100.0f)]
	public float percentage;

	[Header("GridType:")]
	public GridType[] ourGridType;

	#endregion

	#region PRIVATE VARIABLES

	private const string mRow = "MatchMake_RowNumber";
	private const string mColumn = "MatchMake_ColumnNumber";
	private const string mCurrentDifficultyLevel = "MatchMake_DifficultyLevel";

	private int mGridType;
	private int mSizeOfTheGrid;
	private GridAttributes[] mGridAttributes;

	// GamePlay - Property
	private bool mIsLookingForMatch;
	private bool mResetRecentGrid;
	private int mGridIndexOfFirstSelection = -1;

	#endregion

	void Awake(){

		if (Instance == null)
			Instance = this.GetComponent<MatchMakeManager> ();

		if (PlayerPrefs.GetInt (mRow) != baseRow)
			PlayerPrefs.SetInt (mRow, baseRow);

		if (PlayerPrefs.GetInt (mColumn) != baseColumn)
			PlayerPrefs.SetInt (mColumn, baseColumn);
	}

	void Start(){

		//CreateGrid (row,column,focusGridType,percentage);
	}

	void Update(){

		if (!GameManager.Instance.IsGamePaused ()) {
		
			mGridDetection ();
			mGridAppearAnimation ();
		}
	}

	#region PUBLIC FUNCTION

	public void SetGridType(int mGridType){

		this.mGridType = mGridType;
	}

	public void CreateGrid(){

		if (!GameManager.Instance.IsMiniGameRunning ()) {

			GameManager.Instance.SetMiniGameRunning ();

			int mGridIndex = 0;

			float mValueX = 0.0f;
			float mValueY = 0.0f;

			Vector3 mInitialPosition = Vector3.zero;

			mSizeOfTheGrid = PlayerPrefs.GetInt(mRow) * PlayerPrefs.GetInt(mColumn);

			mGridAttributes = new GridAttributes[mSizeOfTheGrid];

			// If : GridSize -> Even Number
			if (mSizeOfTheGrid % 2 == 0) {

				mValueX = ((PlayerPrefs.GetInt(mColumn) / 2.0f) - 0.5f) * distanceAmongEachGridArea;
				mValueY = ((PlayerPrefs.GetInt(mRow) / 2.0f) - 0.5f) * distanceAmongEachGridArea;
				mInitialPosition = new Vector3 (-mValueX, mValueY, 0.0f);
			} else { // If : GridSize -> Odd Number

				mValueX = (PlayerPrefs.GetInt(mColumn)/2) * distanceAmongEachGridArea;
				mValueY = (PlayerPrefs.GetInt(mRow)/2) * distanceAmongEachGridArea;
				mInitialPosition = new Vector3 (-mValueX, mValueY, 0.0f);
			}

			Vector3 mGridPosition = mInitialPosition;

			for (int i = 0; i < PlayerPrefs.GetInt(mRow); i++) {

				for (int j = 0; j < PlayerPrefs.GetInt(mColumn); j++) {

					GameObject newGrid =  Instantiate (gridPrefab, mGridPosition, Quaternion.identity) as GameObject;

					mGridAttributes [mGridIndex].gridObject = newGrid;

					newGrid.name = (mGridIndex++).ToString();
					newGrid.transform.parent = transform;

					mGridPosition = new Vector3 (
						mGridPosition.x + distanceAmongEachGridArea,
						mGridPosition.y,
						mGridPosition.z);
				}

				mGridPosition = new Vector3 (
					mInitialPosition.x, 
					mGridPosition.y - distanceAmongEachGridArea,
					mGridPosition.z);
			}

			mAssignGridAttributes (mGridType,percentage);
		}
	}

	public void IncreaseDifficultyLevel(){
	
		if (PlayerPrefs.GetInt (mCurrentDifficultyLevel, 0) < highestDifficultyLevel) {

			int mCurrentGridSize = PlayerPrefs.GetInt (mRow) * PlayerPrefs.GetInt (mColumn);
			int mNextGridSize = mCurrentGridSize + 4;

			int mLoopCounter = Mathf.Max (PlayerPrefs.GetInt (mRow), PlayerPrefs.GetInt (mColumn)) + 2;

			for (int i = 0; i < mLoopCounter; i++) {

				for (int j = 0; j < mLoopCounter; j++) {

					if (i * j == mNextGridSize) {

						PlayerPrefs.SetInt (mRow, j);
						PlayerPrefs.SetInt (mColumn, i);
						Debug.Log ("New Grid ("
						+ PlayerPrefs.GetInt (mRow)
						+ ","
						+ PlayerPrefs.GetInt (mColumn)
						+ ")");
						break;
					}
				}
			}

			PlayerPrefs.SetInt(
				mCurrentDifficultyLevel,
				PlayerPrefs.GetInt(mCurrentDifficultyLevel) + 1);

			Debug.Log ("Current Difficulty Level : " + PlayerPrefs.GetInt (mCurrentDifficultyLevel));
		}
	}

	public bool IsGameOver(){

		int mSolveCounter = 0;

		for (int i = 0; i < mGridAttributes.Length; i++) {
		
			if (mGridAttributes [i].isGridSolved)
				mSolveCounter++;
		}

		if (mSolveCounter == mGridAttributes.Length)
			return true;
		
		return false;
	}

	#endregion

	#region PRIVATE FUNCTION

	/// <summary>
	/// Ms the assign grid attributes.
	/// </summary>
	/// <param name="mGridType">GridType.</param>
	/// <param name="mAvailableTypesRatio"> 0.0 - 100 </param>
	private void mAssignGridAttributes(int mGridType,float mAvailableTypesRatio){

		int mLocalGridType = 0;
		int mSpecifiedGridType = (int) (((mSizeOfTheGrid / 2) * mAvailableTypesRatio) / 100.0f);

		for (int i = 0; i < mGridAttributes.Length; i++)
			mGridAttributes [i].gridType = -1;


		for (int i = 0; i < (mSizeOfTheGrid / 2); i++) {
		
			int mType1Position = 0;
			bool mIsAssigned = false;

			if (i < mSpecifiedGridType)
				mLocalGridType = mGridType;
			else {

				while (true) {

					mLocalGridType = Random.Range (0, ourGridType.Length);
					if (mLocalGridType != mGridType)
						break;
				}
			}

			while (true) {
			
				mType1Position = Random.Range (0, mSizeOfTheGrid);
				if (mGridAttributes [mType1Position].gridType == -1) {
				
					mGridAttributes [mType1Position].gridType = mLocalGridType;
					mGridAttributes [mType1Position].gridDestroyParticle = ourGridType [mLocalGridType].gridDestroyParticle;

					int mType2Position = 0;
					while (true) {

						mType2Position = Random.Range (0, mSizeOfTheGrid);
						if (mGridAttributes [mType2Position].gridType == -1) {
						
							mGridAttributes [mType2Position].gridType = mLocalGridType;
							mGridAttributes [mType2Position].gridDestroyParticle = ourGridType [mLocalGridType].gridDestroyParticle;

							mIsAssigned = true;
							break;
						}
					}
				}

				if (mIsAssigned)
					break;
			}
		}

	}

	private void mGridDetection(){

		#if UNITY_EDITOR

		if (Input.GetMouseButtonDown(0)) {

			Ray mRayCast = defaultCamera.ScreenPointToRay (Input.mousePosition);
			RaycastHit mRayCastHit;

			if (Physics.Raycast (mRayCast, out mRayCastHit)) {

				if (mRayCastHit.collider.tag == "Grid") {

					int mGridIndex = int.Parse(mRayCastHit.collider.name);

					mGridAttributes [mGridIndex].gridObject.GetComponent<SpriteRenderer> ().sprite =
						ourGridType [mGridAttributes [mGridIndex].gridType].gridSprite;

					mGridAttributes [mGridIndex].nextRotationTime = Time.time;
					mGridAttributes[mGridIndex].rotationCounter = 0;
					mGridAttributes[mGridIndex].isGridAppearAnimationRunning = true;

					if(!mIsLookingForMatch){

						mGridIndexOfFirstSelection = mGridIndex;
						mIsLookingForMatch = true;

						mResetRecentGrid = false;

					}else{

						mGridDisappearAnimation(mGridIndex);
						mIsLookingForMatch = false;
					}
				}
			}
		}

		#else

		if (Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began) {

		Ray mRayCast = defaultCamera.ScreenPointToRay (Input.GetTouch (0).position);
		RaycastHit mRayCastHit;

		if (Physics.Raycast (mRayCast, out mRayCastHit)) {

		if (mRayCastHit.collider.tag == "Grid") {

		int mGridIndex = int.Parse(mRayCastHit.collider.name);

		if(!mIsLookingForMatch){

		mGridAttributes [mGridIndex].gridObject.GetComponent<SpriteRenderer> ().sprite =
		ourGridType [mGridAttributes [mGridIndex].gridType].gridSprite;

		mGridIndexOfFirstSelection = mGridIndex;
		mIsLookingForMatch = true;
		}else{

		if(mGridAttributes[mGridIndexOfFirstSelection].gridType == mGridAttributes[mGridIndex].gridType){

		mGridAttributes[mGridIndexOfFirstSelection].gridObject.SetActive(false);
		mGridAttributes[mGridIndex].gridObject.SetActive(false);
		}else{

		mGridAttributes [mGridIndexOfFirstSelection].gridObject.GetComponent<SpriteRenderer> ().sprite = defaultGridSprite;
		mGridAttributes [mGridIndex].gridObject.GetComponent<SpriteRenderer> ().sprite = defaultGridSprite;
		}

		mGridIndexOfFirstSelection = -1;
		mIsLookingForMatch = false;
		}
		}
		}
		}

		#endif

	}

	private void mGridAppearAnimation(){
	
		if (mGridAttributes != null) {

			for (int i = 0; i < mGridAttributes.Length; i++) {

				if (mGridAttributes [i].isGridAppearAnimationRunning) {

					if (Time.time > mGridAttributes [i].nextRotationTime) {

						mGridAttributes [i].gridObject.transform.Rotate (Vector2.up * rotationAmount );

						mGridAttributes [i].nextRotationTime +=  intervalOnEachRotation/1000.0f;
						mGridAttributes [i].rotationCounter++;

						if (mGridAttributes [i].rotationCounter == numberOfRotation) {

							mGridAttributes [i].isGridAppearAnimationRunning = false;
							if (mResetRecentGrid) {

								mGridAttributes [mGridIndexOfFirstSelection].gridObject.GetComponent<SpriteRenderer> ().sprite = defaultGridSprite;
								mGridAttributes [i].gridObject.GetComponent<SpriteRenderer> ().sprite = defaultGridSprite;
							}
						}
					}
				}
			}
		}
	}

	private void mGridDisappearAnimation(int mGridIndex){

		if(mGridAttributes[mGridIndexOfFirstSelection].gridType == mGridAttributes[mGridIndex].gridType){

			GameObject newDestroyParticle = Instantiate(
				mGridAttributes[mGridIndexOfFirstSelection].gridDestroyParticle,
				mGridAttributes[mGridIndexOfFirstSelection].gridObject.transform.position,
				Quaternion.identity);

			newDestroyParticle.GetComponent<SelfDestroyerWithTimer>().DestroyGameObject(3.0f);

			newDestroyParticle = Instantiate(
				mGridAttributes[mGridIndex].gridDestroyParticle,
				mGridAttributes[mGridIndex].gridObject.transform.position,
				Quaternion.identity);

			newDestroyParticle.GetComponent<SelfDestroyerWithTimer>().DestroyGameObject(3.0f);

			mGridAttributes[mGridIndexOfFirstSelection].isGridSolved = true;
			mGridAttributes[mGridIndex].isGridSolved = true;

			mGridAttributes[mGridIndexOfFirstSelection].gridObject.SetActive(false);
			mGridAttributes[mGridIndex].gridObject.SetActive(false);

			if (IsGameOver ())
				mPostGameOver ();
			
		}else{

			mResetRecentGrid = true;
		}
	}

	private void mPostGameOver(){

		GameManager.Instance.SetMiniGameNotRunning ();
	}

	#endregion
}
