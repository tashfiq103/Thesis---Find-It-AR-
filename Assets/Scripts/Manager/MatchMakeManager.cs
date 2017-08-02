using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMakeManager : MonoBehaviour {

	public static MatchMakeManager Instance;

	#region PUBLIC VARIABLES

	public int sizeOfTheGrid;
	public float distanceAmongEachGridArea;
	public GameObject gridAreaPrefab;

	#endregion

	#region PRIVATE VARIABLES

	#endregion

	void Awake(){

		if (Instance == null)
			Instance = this.GetComponent<MatchMakeManager> ();
	}

	void Start(){

		CreateGrid ();
	}

	#region PUBLIC FUNCTION

	public void CreateGrid(){

		float mValue = 0.0f;
		Vector3 mInitialPosition = Vector3.zero;

		// If : GridSize -> Even Number
		if (sizeOfTheGrid % 2 == 0) {

			mValue = ((sizeOfTheGrid / 2.0f) - 0.5f) * distanceAmongEachGridArea;
			mInitialPosition = new Vector3 (-mValue, -mValue, 0.0f);
		} else { // If : GridSize -> Odd Number
		
			mValue = ((int)(sizeOfTheGrid / 2)) * distanceAmongEachGridArea;
			mInitialPosition = new Vector3 (-mValue, mValue, 0.0f);
		}

		Vector3 mGridPosition = mInitialPosition;

		for (int i = 0; i < sizeOfTheGrid; i++) {

			for (int j = 0; j < sizeOfTheGrid; j++) {

				GameObject newGrid =  Instantiate (gridAreaPrefab, mGridPosition, Quaternion.identity) as GameObject;

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
	}

	#endregion

	#region PRIVATE FUNCTION

	#endregion
}
