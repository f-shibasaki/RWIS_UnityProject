using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] // 生成する7種類のブロックを保存
    Block[] Blocks;

    List<int> numbers = new List<int>();
    Queue<int> RandomBag = new Queue<int>();

    // ブロックをランダムに選択
    Block GetRandomBlock()
    {
        if (RandomBag.Count <= 0)
        {
            for (int i = 0; i < Blocks.Length; i++)
            {
                numbers.Add(i);
            }
            while (numbers.Count > 0)
            {
                int index = Random.Range(0, numbers.Count);
                RandomBag.Enqueue(numbers[index]);
                numbers.RemoveAt(index);
            }
        }

        if (Blocks[RandomBag.Peek()])
        {
            return Blocks[RandomBag.Dequeue()];
        }
        else
        {
            return null;
        }
    }

    // ブロックを生成
    public Block SpawnBlock ()
    {
        Block block = Instantiate(GetRandomBlock(), transform.position, Quaternion.identity, transform.parent);

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
