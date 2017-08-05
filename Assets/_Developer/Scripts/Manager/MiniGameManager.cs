using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameManager : MonoBehaviour {

	public static MiniGameManager Instance;

	#region CUSTOM DATA Type

	[System.Serializable]
	public struct Game
	{
		public GameObject miniGameManager;
		[RangeAttribute(0,10)]
		public int baseDifficultyLevel;	
		public UnityEvent createGame;
	}

	#endregion

	#region PUBLIC Variables

	public Game[] miniGame;

	#endregion

	#region PRIVATE Variables

	private string[] mGamePerformancePreference;

	#endregion

	void Awake(){

		if (Instance == null)
			Instance = this.GetComponent<MiniGameManager> ();

		mGeneratePreference ();
	}

	#region PRIVATE - Function

	private void mGeneratePreference(){

		mGamePerformancePreference = new string[miniGame.Length];

		for (int i = 0; i < miniGame.Length; i++) {

			mGamePerformancePreference [i] = "GamePerformance[" + i + "]";
		}
	}

	#endregion

	public void CreateGame(){


	}
}
