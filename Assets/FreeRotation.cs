using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreeRotation : MonoBehaviour
{
	public float RotationSpeed = 1f;

    void Update()
    {
		if (Input.GetMouseButton(0))
			transform.eulerAngles += new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * RotationSpeed * Time.deltaTime;
			//transform.eulerAngles += new Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse Y"), 0) * RotationSpeed * Time.deltaTime;
	}
}
