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

    
    /********************************
     * 初期化系関数
     ********************************/

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

    
    /********************************
     * 重なり判定
     ********************************/

    // 1マス落下できるかどうか
    public bool canFall (Field field)
    {
        // テトリミノを構成するブロックのうち1つでも落下不可能ならばテトリミノは落下不可能
        foreach (Block elem in blocks)
        {
            // 1つ下に移動して重なるかどうか
            if (elem.is_overlap (field, this.x, this.y - 1))
            {
                return false;
            }
        }
        return true;
    }

    // 1マス左に移動できるかどうか
    public bool canMoveLeft (Field field)
    {
        // テトリミノを構成するブロックのうち1つでも移動不可能ならばテトリミノは移動不可能
        foreach (Block elem in blocks)
        {
            // 1つ左に移動して重なるかどうか
            if (elem.is_overlap (field, this.x - 1, this.y))
            {
                return false;
            }
        }
        return true;
    }

    // 1マス右に移動できるかどうか
    public bool canMoveRight (Field field)
    {
        // テトリミノを構成するブロックのうち1つでも移動不可能ならばテトリミノは移動不可能
        foreach (Block elem in blocks)
        {
            // 1つ右に移動して重なるかどうか
            if (elem.is_overlap (field, this.x + 1, this.y))
            {
                return false;
            }
        }
        return true;
    }

    /********************************
     * 移動系関数
     ********************************/

    // 1マス落下
    public void fall ()
    {
        // パラメーター更新
        -- this.y;

        // 位置更新
        updatePos ();
    }

    // 左に移動
    public void moveLeft ()
    {
        // パラメーター更新
        -- this.x;

        // 位置更新
        updatePos ();
    }

    // 右に移動
    public void moveRight ()
    {
        // パラメーター更新
        ++ this.x;

        // 位置更新
        updatePos ();
    }

    // 左回転
    public void rotateLeft ()
    {
        foreach (Block elem in blocks)
        {
            elem.rotateLeft ();
        }
    }

    // 右回転
    public void rotateRight ()
    {
        foreach (Block elem in blocks)
        {
            elem.rotateRight ();
        }
    }

    /********************************
     * 設置
     ********************************/

    // 設置
    public void place (Field field)
    {
        foreach (Block elem in blocks)
        {
            elem.place (field, this.x , this.y);
        }
    }

    /********************************
     * ('-'v)
     ********************************/

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
