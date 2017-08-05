using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

	[Header("Allowing Rotation:")]
	public bool rotateOverX_Axis;
	public bool rotateOverY_Axis;
	public bool rotateOverZ_Axis;

	[Header("Adjusting Rotation Speed:")]
	public bool rotationClockWise;
	[Range(0.0f,180.0f)]
	public float rotationSpeed;
	
	// Update is called once per frame
	void Update () {
	
		if (rotateOverX_Axis) {

			if (rotationClockWise) {

				gameObject.transform.Rotate (Vector3.right * rotationSpeed);
			} else {

				gameObject.transform.Rotate (Vector3.left * rotationSpeed);
			}
		}

		if (rotateOverY_Axis) {

			if (rotationClockWise) {

				gameObject.transform.Rotate (Vector3.up * rotationSpeed);
			} else {

				gameObject.transform.Rotate (Vector3.down * rotationSpeed);
			}
		}

		if (rotateOverZ_Axis) {

			if (rotationClockWise) {

				gameObject.transform.Rotate (Vector3.forward * rotationSpeed);
			} else {

				gameObject.transform.Rotate (Vector3.back * rotationSpeed);
			}
		}
	}
}
