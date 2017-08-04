using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager Instance;

	private static bool mIsGamePaused;
	private static bool mIsGameRunning;

	void Awake(){

		if (Instance == null)
			Instance = this.GetComponent<GameManager> ();
	}

	#region Game Pause/Play - Callback

	public bool IsGamePaused(){
	
		return mIsGamePaused;
	}

	public void SetGamePaused(){

		mIsGamePaused = true;
	}

	public void SetGameResume(){

		mIsGamePaused = false;
	}

	#endregion

	#region MINI - Games Callback

	public bool IsMiniGameRunning(){

		return mIsGameRunning;
	}

	public void SetMiniGameRunning(){
	
		mIsGameRunning = true;
	}
		
	public void SetMiniGameNotRunning(){
	
		mIsGameRunning = false;
	}

	#endregion
}
