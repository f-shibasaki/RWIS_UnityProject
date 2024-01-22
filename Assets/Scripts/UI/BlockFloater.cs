using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockFloater : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 v = new Vector3(0.0f, 0.0f, 0.0f);
    private 

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        v.x = UnityEngine.Random.Range(0.85f, -0.85f);
        v.y = UnityEngine.Random.Range(10.645f, 8.98f);
        rb.AddForce(v, ForceMode.Force);
    }

    void Update()
    {
        gameObject.transform.Rotate(new Vector3(UnityEngine.Random.Range(20, -20), UnityEngine.Random.Range(20, -20), UnityEngine.Random.Range(20, -20)) * Time.deltaTime);
    }
}
