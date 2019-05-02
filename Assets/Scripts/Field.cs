using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Block[,] data = new Block[10,22];

    // getter
    public Block getblock (int x, int y)
    {
        return this.data [x, y];
    }

    // setter
    public void setblock (int x, int y, Block block)
    {
        this.data [x, y] = block;
    }

    // (x,y)がブロックかどうか(壁もブロックとする)
    public bool isblock (int x, int y)
    {
        if (x < 0 || y < 0 || x >= 10 || y >= 22)
        {
            return true;
        }
        return data [x, y] != null;
    }

    public void clear_line_1 (int y_)
    {
        int y = y_;

        // ブロックの破棄
        for (int x = 0; x < 10; ++ x)
        {
            Destroy (data [x, y].gameObject);
        }
        
        // 一つ下に下げる
        for (; y < 21; ++ y)
        {
            for (int x = 0; x < 10; ++ x)
            {
                data [x, y] = data [x, y + 1];
                if (data [x, y])
                {
                    data [x, y].moveDown ();
                }
            }
        }
    }

    // ライン消去
    // 戻り値: 消したライン数
    public int clear_lines ()
    {
        int res = 0;
        for (int y = 0; y < 22; ++ y)
        {
            for (int x = 0; x < 10; ++ x)
            {
                // ブロックが無いところがあったら消せないので次
                if (data [x, y] == null)
                {
                    goto nextline;
                }
            }
            clear_line_1 (y --);
            ++ res;

            nextline:
            ;
        }
        return res;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
