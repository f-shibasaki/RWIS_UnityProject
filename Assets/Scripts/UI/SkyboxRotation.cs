using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkyboxRotation : MonoBehaviour
{
    [SerializeField] float anglePerFrame = 0.01f;
    float rotation = 0.0f;

    void Start()
    {
        rotation = RenderSettings.skybox.GetFloat("_Rotation");
    }

    void Update()
    {
        rotation += anglePerFrame;
        if (rotation >= 360.0f)
        {
            rotation -= 360.0f;
        }
        RenderSettings.skybox.SetFloat("_Rotation", rotation);
    }
}
