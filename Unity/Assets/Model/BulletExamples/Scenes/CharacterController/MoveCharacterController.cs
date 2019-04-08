using UnityEngine;
using System.Collections;
using BulletUnity;

public class MoveCharacterController : MonoBehaviour {

    public BCharacterController go;

	// Update is called once per frame
	void FixedUpdate () {
	    if (Input.GetKey(KeyCode.UpArrow))
        {
            go.Move(go.transform.forward * Time.fixedDeltaTime * 5);
        } else if (Input.GetKey(KeyCode.DownArrow))
        {
            go.Move(-go.transform.forward * Time.fixedDeltaTime * 5);
        } else
        {
            go.Move(Vector3.zero);
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            go.Rotate(-Mathf.PI * .01f);
        } else if (Input.GetKey(KeyCode.RightArrow))
        {
            go.Rotate(Mathf.PI * .01f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            go.Jump();
        }
	}
}
