using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetrimino : MonoBehaviour
{
    // ブロックの元になるprefab
    [SerializeField]
    private GameObject blockPrefab_;

    // テトリミノを構成するブロックの塊
    private Block[] blocks;

    // テトリミノの位置
    private int x;
    private int y;
    
    // 位置を変更したあとに呼び出して更新するためのもの
    private void updatePos ()
    {
        var transform = this.GetComponent <RectTransform> ();

        // 更新
        transform.anchoredPosition = new Vector2(this.x * transform.rect.width / 4, this.y * transform.rect.height / 4);
    }

    // ブロックを1つ生成する
    private Block generateBlock (int x, int y, int color)
    {
        var block = Instantiate <GameObject> (blockPrefab_);
        block.transform.SetParent (this.GetComponent <RectTransform> (), false);
        var res = block.GetComponent <Block> ();
        res.init (x, y, color);
        return res;
    }


    public void init (int x, int y, int color)
    {
        // パラメーター更新
        this.x = x;
        this.y = y;

        // 位置更新
        updatePos ();

        // ブロック生成
        if (color == 1)
        {
            // Jミノ 青
            blocks = new Block[] {
                generateBlock (0, 1, color),
                generateBlock (1, 1, color),
                generateBlock (2, 1, color),
                generateBlock (0, 2, color)
            };
        }
        else if (color == 2)
        {
            // Iミノ シアン
            blocks = new Block[] {
                generateBlock (0, 2, color),
                generateBlock (1, 2, color),
                generateBlock (2, 2, color),
                generateBlock (3, 2, color)
            };
        }
        else if (color == 3)
        {
            // Sミノ 緑
            blocks = new Block[] {
                generateBlock (0, 0, color),
                generateBlock (1, 0, color),
                generateBlock (1, 1, color),
                generateBlock (2, 1, color)
            };
        }
        else if (color == 4)
        {
            // Lミノ オレンジ
            blocks = new Block[] {
                generateBlock (0, 1, color),
                generateBlock (1, 1, color),
                generateBlock (2, 1, color),
                generateBlock (2, 2, color)
            };
        }
        else if (color == 5)
        {
            // Tミノ 紫
            blocks = new Block[] {
                generateBlock (0, 1, color),
                generateBlock (1, 1, color),
                generateBlock (2, 1, color),
                generateBlock (1, 2, color)
            };
        }
        else if (color == 6)
        {
            // Zミノ 赤
            blocks = new Block[] {
                generateBlock (1, 1, color),
                generateBlock (2, 1, color),
                generateBlock (0, 2, color),
                generateBlock (1, 2, color)
            };
        }
        else if (color == 7)
        {
            // Oミノ 黄
            blocks = new Block[] {
                generateBlock (1, 1, color),
                generateBlock (2, 1, color),
                generateBlock (1, 2, color),
                generateBlock (2, 2, color)
            };
        }
        else
        {
            Debug.Log ("unknown color id");
        }
    }

    // 1マス落下できるかどうか
    public bool canfall (Field field)
    {
        // テトリミノを構成するブロックのうち1つでも落下不可能ならばテトリミノは落下不可能
        foreach (Block elem in blocks)
        {
            if (! elem.canfall (field, this.x, this.y))
            {
                return false;
            }
        }
        return true;
    }

    // 1マス落下
    public void fall ()
    {
        // パラメーター更新
        -- this.y;

        // 位置更新
        updatePos ();
    }

    // 設置
    public void place (Field field)
    {
        foreach (Block elem in blocks)
        {
            elem.place (field, this.x , this.y);
        }
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
