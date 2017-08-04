using UnityEngine;
using Vuforia;

public class AREventHandler : MonoBehaviour,ITrackableEventHandler {

	private TrackableBehaviour mTrackableBehaviour;

	// Use this for initialization
	void Start () {

		mTrackableBehaviour = GetComponent<TrackableBehaviour> ();
		if (mTrackableBehaviour)
			mTrackableBehaviour.RegisterTrackableEventHandler (this);
	}

	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			OnTrackingFound();
		}
		else
		{
			OnTrackingLost();
		}
	}

	private void OnTrackingFound(){

		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
		Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

		// Enable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = true;
		}

		// Enable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = true;
		}

		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " found");
	}

	private void OnTrackingLost(){

		Renderer[] rendererComponents = GetComponentsInChildren<Renderer>(true);
		Collider[] colliderComponents = GetComponentsInChildren<Collider>(true);

		// Disable rendering:
		foreach (Renderer component in rendererComponents)
		{
			component.enabled = false;
		}

		// Disable colliders:
		foreach (Collider component in colliderComponents)
		{
			component.enabled = false;
		}

		Debug.Log("Trackable " + mTrackableBehaviour.TrackableName + " lost");
	}
}
