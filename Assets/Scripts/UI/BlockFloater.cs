using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFloater : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 v = new Vector3(0.0f, 0.0f, 0.0f);

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();

        v.x = Random.Range(0, 15);
        v.y = Random.Range(0, 25);
        v.z = 0.0f;
        gameObject.transform.position = v;
        
        v.x = Random.Range(0, 360);
        v.y = Random.Range(0, 360);
        v.z = Random.Range(0, 360);
        gameObject.transform.Rotate(v);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        v.x = Random.Range(0.85f, -0.85f);
        v.y = Random.Range(10.645f, 8.98f);
        v.z = 0.0f;
        rb.AddForce(v, ForceMode.Force);
    }

    void Update()
    {
        gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(20, -20), UnityEngine.Random.Range(20, -20), UnityEngine.Random.Range(20, -20)) * Time.deltaTime);
    }
}
