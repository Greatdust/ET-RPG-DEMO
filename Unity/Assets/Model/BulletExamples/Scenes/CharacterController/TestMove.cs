using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMove : MonoBehaviour
{
    public CharacterController go;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            go.Move(go.transform.forward * Time.fixedDeltaTime * 5);
        }
        else if (Input.GetKey(KeyCode.DownArrow))
        {
            go.Move(-go.transform.forward * Time.fixedDeltaTime * 5);
        }
        else
        {
            go.Move(Vector3.zero);
        }
    }
}
