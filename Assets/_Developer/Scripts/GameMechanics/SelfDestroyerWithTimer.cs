using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelfDestroyerWithTimer : MonoBehaviour {

	private float timeToDestroy;
	private bool isTimerOn;

	// Update is called once per frame
	void Update () {

		if (!GameManager.Instance.IsGamePaused ()) {
		
			if (isTimerOn) {

				if (Time.time > timeToDestroy)
					Destroy (gameObject);
			}
		}
	}

	public void DestroyGameObject(float mTimeToDestroy){

		timeToDestroy = Time.time + mTimeToDestroy;
		isTimerOn = true;
	}
}
