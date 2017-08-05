using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARTargetHandler : MonoBehaviour {

	#region PUBLIC Variables

	[Header("GameObject Reference:")]
	public Transform targetLocker;
	public SpriteRenderer progressBar;

	[Header("TargetLocker Property:")]
	[Range(0.0f,360.0f)]
	public float rotationSpeed;
	[Range(0.0f,1000.0f)]
	public float rotationInterval;

	[Space]
	[Range(1,10)]
	public int progressbarLength;
	[Range(0.0f,5.0f)]
	public float progressbarSpeed;

	#endregion

	#region PRIVATE Variables

	private bool mIsFoundObject;
	private bool mIsCompletedTheCycle;
	private bool mIsCanceledTheCycle;

	private float mNextRotationTime;

	#endregion

	// Use this for initialization
	void Start () {

		rotationInterval /= 1000.0f;

	}

	void Update(){

		if (!GameManager.Instance.IsGamePaused()) {

			if (mIsFoundObject) {

				if (!mIsCompletedTheCycle) {

					if (Time.time > mNextRotationTime) {

						targetLocker.Rotate (Vector3.forward * rotationSpeed);
						mNextRotationTime = Time.time + rotationInterval;
					}

					if (progressBar.size.x < progressbarLength) {

						Vector2 sizeOfProgressBar = progressBar.size;

						float mActualDistanceToCover = 0.0f;

						if ((progressBar.size.x + progressbarSpeed) > progressbarLength)
							mActualDistanceToCover = progressbarLength - progressBar.size.x;
						else
							mActualDistanceToCover = progressbarSpeed;

						sizeOfProgressBar = new Vector2 (
							sizeOfProgressBar.x + mActualDistanceToCover,
							sizeOfProgressBar.y);

						progressBar.size = sizeOfProgressBar;

						if (progressBar.size.x == progressbarLength) {

							progressBar.size = new Vector2 (1, 1); 
							mPostProcessOfObjectDetection ();
						}
					}
				} else {
				

				}
			}
		}
	}

	public void FoundObject(){

		targetLocker.gameObject.SetActive (true);
		progressBar.gameObject.SetActive (true);

		mIsFoundObject = true;
		mIsCompletedTheCycle = false;
		mIsCanceledTheCycle = false;
	}

	public void LostObject(){

		mIsCanceledTheCycle = !mIsCompletedTheCycle;
		mIsFoundObject = false;
	}

	private void mPostProcessOfObjectDetection(){

		mIsCompletedTheCycle = true;

		targetLocker.gameObject.SetActive (false);
		progressBar.gameObject.SetActive (false);
	
		MiniGameManager.Instance.CreateGame ();
	}

	#region CALLBACK

	public bool IsObjectFound(){

		return mIsFoundObject;
	}

	public bool IsObjectDetectionComplete(){

		return mIsCompletedTheCycle;
	}

	public bool IsObjectDetectionCanceled(){

		return mIsCanceledTheCycle;
	}

	#endregion
}
