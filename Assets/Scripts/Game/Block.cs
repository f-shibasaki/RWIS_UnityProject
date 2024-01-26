using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [SerializeField]
    private bool canRotate = true;

    // ブロックの移動
    void Move(Vector3 moveDirection)
    {
        transform.position += moveDirection;
    }

    public void MoveLeft()
    {
        Move(new Vector3(-1, 0, 0));
    }

    public void MoveRight()
    {
        Move(new Vector3(1, 0, 0));
    }

    public void MoveUp()
    {
        Move(new Vector3(0, 1, 0));
    }

    public void MoveDown()
    {
        Move(new Vector3(0, -1, 0));
    }

    // ブロックの回転
    public void RotateRight()
    {
        if (canRotate)
        {
            if (transform.name == "Block_I(Clone)")
            {
                transform.Rotate(0, 0, -90);
                if (transform.rotation.eulerAngles.z == 0.00f)
                {
                    transform.position += new Vector3(0, 1, 0);
                }
                else if (transform.rotation.eulerAngles.z == 90.00f)
                {
                    transform.position += new Vector3(-1, 0, 0);
                }
                else if (transform.rotation.eulerAngles.z == 180.00f)
                {
                    transform.position += new Vector3(0, -1, 0);
                }
                else if (transform.rotation.eulerAngles.z == 270.00f)
                {
                    transform.position += new Vector3(1, 0, 0);
                }
            }
            else
            {
                transform.Rotate(0, 0, -90);
            }
        }
    }

    public void RotateLeft()
    {
        if (canRotate)
        {
            if (transform.name == "Block_I(Clone)")
            {
                transform.Rotate(0, 0, 90);
                if (transform.rotation.eulerAngles.z == 0.00f)
                {
                    transform.position += new Vector3(-1, 0, 0);
                } else if (transform.rotation.eulerAngles.z == 90.00f)
                {
                    transform.position += new Vector3(0, -1, 0);
                } else if (transform.rotation.eulerAngles.z == 180.00f)
                {
                    transform.position += new Vector3(1, 0, 0);
                } else if (transform.rotation.eulerAngles.z == 270.00f)
                {
                    transform.position += new Vector3(0, 1, 0);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }
        }
    }

    public void RotateUp()
    {
        if (canRotate)
        {
            transform.Rotate(180, 0, 0);
        }
    }
}
