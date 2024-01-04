using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class MoveWithGyro : MonoBehaviour
{
    // ジャイロセンサーに応じて、targetポジションに対してy軸方向に弧を描くようにカメラが動く
    
    [SerializeField] private Vector3 targetPosition;
    [SerializeField] private float speed = 1f;

    [SerializeField] private float minAngle = -15f;
    [SerializeField] private float maxAngle = 15f;
    
    // カメラ
    private Camera _camera;
    private Vector3 _initialPosition;
    // 初期ジャイロ
    private Quaternion _initialPose;
    
    void Start()
    {
        _camera = GetComponent<Camera>();
        _initialPosition = transform.position;
        
        ResetGyro();
    }

    void Update()
    {
        Quaternion inputGyro = Input.gyro.attitude;
        Quaternion newPose = new Quaternion(-inputGyro.x, -inputGyro.y, inputGyro.z, inputGyro.w);
        Vector3 angles = (Quaternion.Inverse(_initialPose) * newPose).eulerAngles;
        
        float moveAngle = angles.y;
        // 0 ~ 360 -> -180 ~ 180
        if (moveAngle > 180)
        {
            moveAngle -= 360;
        }
        moveAngle *= maxAngle / 90;
        moveAngle = Mathf.Clamp(moveAngle, minAngle, maxAngle);
        
#if UNITY_EDITOR
        moveAngle = Input.GetAxis("Horizontal") * speed;
#endif

        // ジャイロセンサーの値に応じてカメラをy軸周りに回転
        transform.position = _initialPosition;
        transform.RotateAround(targetPosition, Vector3.up, -moveAngle);
        transform.LookAt(targetPosition);
    }
    
    public void ResetGyro()
    {
        Quaternion initialGyro = Input.gyro.attitude;
        _initialPose = new Quaternion(-initialGyro.x, -initialGyro.y, initialGyro.z, initialGyro.w);
    }
}
