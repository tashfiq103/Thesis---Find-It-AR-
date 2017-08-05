using UnityEngine;
using System.Collections;

public class GlowEffect : MonoBehaviour {


	[Range(0.0f,100.0f)]
	public float scalePercentage;
	[Range(0.0000f,1.0f)]
	public float scalingSpeed;

	private float lowerBoundaryOfScaleFactor;
	private float upperBoundaryOfScaeFactor;

	private bool _isGoingForLowerBoundary;

	private RectTransform _rectTransform;

	// Use this for initialization
	void Start () {

		if (gameObject.GetComponent<RectTransform> ())
			_rectTransform = gameObject.GetComponent<RectTransform> ();

		lowerBoundaryOfScaleFactor = transform.localScale.x - (transform.localScale.x * scalePercentage) / 100.0f;
		upperBoundaryOfScaeFactor = transform.localScale.x + (transform.localScale.x * scalePercentage) / 100.0f;
	}
	
	// Update is called once per frame
	void Update () {

		if (gameObject.GetComponent<RectTransform> ()) {

			if (_isGoingForLowerBoundary && _rectTransform.localScale.x >= lowerBoundaryOfScaleFactor) {

				_rectTransform.localScale = new Vector3 (_rectTransform.localScale.x - scalingSpeed, _rectTransform.localScale.y - scalingSpeed, _rectTransform.localScale.z - scalingSpeed);

				if (_rectTransform.localScale.x < lowerBoundaryOfScaleFactor) {

					_isGoingForLowerBoundary = false;
				}
			} else {

				_rectTransform.localScale = new Vector3 (_rectTransform.localScale.x + scalingSpeed, _rectTransform.localScale.y + scalingSpeed, _rectTransform.localScale.z + scalingSpeed);

				if (_rectTransform.localScale.x > upperBoundaryOfScaeFactor) {

					_isGoingForLowerBoundary = true;
				}

			}
		} else {

			if (_isGoingForLowerBoundary && transform.localScale.x >= lowerBoundaryOfScaleFactor) {

				transform.localScale = new Vector3 (transform.localScale.x - scalingSpeed, transform.localScale.y - scalingSpeed, transform.localScale.z - scalingSpeed);

				if (transform.localScale.x < lowerBoundaryOfScaleFactor) {

					_isGoingForLowerBoundary = false;
				}
			} else {

				transform.localScale = new Vector3 (transform.localScale.x + scalingSpeed, transform.localScale.y + scalingSpeed, transform.localScale.z + scalingSpeed);

				if (transform.localScale.x > upperBoundaryOfScaeFactor) {

					_isGoingForLowerBoundary = true;
				}

			}
		}
	}
}
