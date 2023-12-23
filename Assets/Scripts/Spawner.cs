using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] // 生成する7種類のブロックを保存
    Block[] Blocks;

    // ブロックをランダムに選択
    Block GetRandomBlock()
    {
        int i = Random.Range(0, Blocks.Length);

        if (Blocks[i]) 
        {
            return Blocks[i];
        } 
        else
        {
            return null;
        }
    }

    // ブロックを生成
    public Block SpawnBlock ()
    {
        Block block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity);

        if (block)
        {
            return block;
        } 
        else
        {
            return null;
        }
    }
}
