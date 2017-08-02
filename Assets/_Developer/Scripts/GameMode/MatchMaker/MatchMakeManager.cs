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
	}

	[System.Serializable]
	public struct GridAttributes
	{
		public int gridType;

		[HideInInspector]
		public GameObject gridObject;
	}

	#endregion

	#region PUBLIC VARIABLES

	public Transform ARCamera;

	[Header("Grid Property:")]
	[Range(1,10)]
	public float distanceAmongEachGridArea;
	public GameObject gridPrefab;
	[Header("--------------")]
	public int row;
	public int column;
	public int focusGridType;
	[Range(0.0f,100.0f)]
	public float percentage;

	[Header("GridType:")]
	public GridType[] ourGridType;

	#endregion

	#region PRIVATE VARIABLES

	private int mSizeOfTheGrid;
	public GridAttributes[] mGridAttributes;

	#endregion

	void Awake(){

		if (Instance == null)
			Instance = this.GetComponent<MatchMakeManager> ();
	}

	void Start(){

		CreateGrid (row,column,focusGridType,percentage);
	}

	void Update(){

		transform.LookAt (ARCamera);
	}

	#region PUBLIC FUNCTION

	public void CreateGrid(int row, int column,int mGridType,float mAvailableTypesRatio){

		int mGridIndex = 0;

		float mValueX = 0.0f;
		float mValueY = 0.0f;

		Vector3 mInitialPosition = Vector3.zero;

		mSizeOfTheGrid = row * column;

		mGridAttributes = new GridAttributes[mSizeOfTheGrid];

		// If : GridSize -> Even Number
		if (mSizeOfTheGrid % 2 == 0) {

			mValueX = ((column / 2.0f) - 0.5f) * distanceAmongEachGridArea;
			mValueY = ((row / 2.0f) - 0.5f) * distanceAmongEachGridArea;
			mInitialPosition = new Vector3 (-mValueX, mValueY, 0.0f);
		} else { // If : GridSize -> Odd Number

			mValueX = (column/2) * distanceAmongEachGridArea;
			mValueY = (row/2) * distanceAmongEachGridArea;
			mInitialPosition = new Vector3 (-mValueX, mValueY, 0.0f);
		}

		Vector3 mGridPosition = mInitialPosition;

		for (int i = 0; i < row; i++) {

			for (int j = 0; j < column; j++) {

				GameObject newGrid =  Instantiate (gridPrefab, mGridPosition, Quaternion.identity) as GameObject;

				mGridAttributes [mGridIndex].gridObject = newGrid;

				newGrid.name = "Grid (" + (mGridIndex++) + ")";
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

		mAssignGridAttributes (mGridType,mAvailableTypesRatio);
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
					mGridAttributes [mType1Position].gridObject.GetComponent<SpriteRenderer> ().sprite =
						ourGridType [mLocalGridType].gridSprite;

					int mType2Position = 0;
					while (true) {

						mType2Position = Random.Range (0, mSizeOfTheGrid);
						if (mGridAttributes [mType2Position].gridType == -1) {
						
							mGridAttributes [mType2Position].gridType = mLocalGridType;
							mGridAttributes [mType2Position].gridObject.GetComponent<SpriteRenderer> ().sprite =
								ourGridType [mLocalGridType].gridSprite;

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

	#endregion
}
