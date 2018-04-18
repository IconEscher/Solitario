using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flip : MonoBehaviour {

	public int fps = 60;
	public float rotateDegreePerSecond = 180f;
	public bool isFaceUp = false;

	const float FLIP_LIMIT_DEGREE = 180f;

	float waitTime;
	bool isAnimationProcessing = false;

	// Use this for initialization
	void Start () {
		waitTime = 1.0f / fps;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void OnMouseDown ()
	{
		if (isAnimationProcessing) {
			return;
		}

		StartCoroutine (flip ());
	}

		IEnumerator flip ()
		{
			isAnimationProcessing = true;

			bool done = false;
			while (!done)
			{
				float degree = rotateDegreePerSecond * Time.deltaTime;
				if (isFaceUp) {
					degree = -degree;
				}

			transform.Rotate( new Vector3(0, degree, 0));

			if (FLIP_LIMIT_DEGREE < transform.eulerAngles.y) {
				done = true;
			}

           // isFaceUp = !isFaceUp;

				yield return new WaitForSeconds(waitTime);
			}
		}
	}

