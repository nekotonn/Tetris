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

    // テトリミノの種類
    private char minotype;

    // テトリミノの位置
    private int x;
    private int y;

    // テトリミノの回転角度
    // 0:0°, 1:右90°, 2:180°, 3:左90°
    private int radius;

    
    // 位置を変更したあとに呼び出して更新するためのもの
    private void updatePos ()
    {
        var transform = this.GetComponent <RectTransform> ();

        // 更新
        transform.anchoredPosition = new Vector2(this.x * transform.rect.width / 4, this.y * transform.rect.height / 4);
    }

    // for debug
    public int getRadius ()
    {
        return radius;
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

        this.radius = 0;

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
            minotype = 'J';
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
            minotype = 'I';
        }
        else if (color == 3)
        {
            // Sミノ 緑
            blocks = new Block[] {
                generateBlock (0, 1, color),
                generateBlock (1, 1, color),
                generateBlock (1, 2, color),
                generateBlock (2, 2, color)
            };
            minotype = 'S';
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
            minotype = 'L';
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
            minotype = 'T';
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
            minotype = 'Z';
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
            minotype = 'O';
        }
        else
        {
            Debug.Log ("unknown color id");
        }
    }

    
    /********************************
     * 重なり判定
     ********************************/

    // 接触しているかどうか
    public bool is_hit (Field field)
    {
        foreach (Block elem in blocks)
        {
            if (elem.is_hit (field, this.x, this.y))
            {
                return true;
            }
        }
        return false;
    }

    // 1マス落下できるかどうか
    public bool canFall (Field field)
    {
        -- this.y;
        var res = ! is_hit (field);
        ++ this.y;
        return res;
    }

    // 1マス左に移動できるかどうか
    public bool canMoveLeft (Field field)
    {
        -- this.x;
        var res = ! is_hit (field);
        ++ this.x;
        return res;
    }

    // 1マス右に移動できるかどうか
    public bool canMoveRight (Field field)
    {
        ++ this.x;
        var res = ! is_hit (field);
        -- this.x;
        return res;
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

    // 強制左回転
    private void rotateLeft_force ()
    {
        // 回転処理
        foreach (Block elem in blocks)
        {
            elem.rotateLeft ();
        }

        // 角度更新
        this.radius = (this.radius == 0) ? 3 : this.radius - 1;
    }

    // 強制右回転
    private void rotateRight_force ()
    {
        // 回転処理
        foreach (Block elem in blocks)
        {
            elem.rotateRight ();
        }

        // 角度更新
        this.radius = (this.radius == 3) ? 0 : this.radius + 1;
    }


    // 左回転
    public bool rotateLeft (Field field)
    {
        rotateLeft_force ();

        // Super Rotation System
        if (is_hit (field))
        {
            // 補正前の座標を記憶
            int backup_x = this.x;
            int backup_y = this.y;

            // JSLTZミノについて
            // Oミノは必ず回転に成功するので考慮しない
            if (minotype != 'I')
            {
                // 第1段階
                if (this.radius == 0)
                {
                    ++ this.x;
                }
                else if (this.radius == 1)
                {
                    -- this.x;
                }
                else if (this.radius == 2)
                {
                    -- this.x;
                }
                else if (this.radius == 3)
                {
                    ++ this.x;
                }

                if (is_hit (field))
                {
                    // 第2段階
                    if (this.radius == 0 || this.radius == 2)
                    {
                        -- this.y;
                    }
                    else if (this.radius == 1 || this.radius == 3)
                    {
                        ++ this.y;
                    }

                    if (is_hit (field))
                    {
                        // 第3段階
                        this.x = backup_x;
                        this.y = backup_y;

                        if (this.radius == 0 || this.radius == 2)
                        {
                            this.y += 2;
                        }
                        else if (this.radius == 1 || this.radius == 3)
                        {
                            this.y -= 2;
                        }

                        if (is_hit (field))
                        {
                            // 第4段階
                            if (this.radius == 0)
                            {
                                ++ this.x;
                            }
                            else if (this.radius == 1)
                            {
                                -- this.x;
                            }
                            else if (this.radius == 2)
                            {
                                -- this.x;
                            }
                            else if (this.radius == 3)
                            {
                                ++ this.x;
                            }

                            if (is_hit (field))
                            {
                                // 失敗 => 元に戻す
                                
                                // 場所を元に戻す
                                this.x = backup_x;
                                this.y = backup_y;
                                
                                // 回転も元に戻す
                                rotateRight_force ();

                                Debug.Log ("回転失敗");
                                return false;
                            }
                        }
                    }
                }
            }
            // Iミノ
            else if (minotype == 'I')
            {
                // 第1段階
                if (this.radius == 0)
                {
                    this.x += 2;
                }
                else if (this.radius == 1)
                {
                    ++ this.x;
                }
                else if (this.radius == 2)
                {
                    ++ this.x;
                }
                else if (this.radius == 3)
                {
                    -- this.x;
                }

                if (is_hit (field))
                {
                    // 第2段階
                    if (this.radius == 0)
                    {
                        this.x -= 3;
                    }
                    else if (this.radius == 1)
                    {
                        this.x -= 3;
                    }
                    else if (this.radius == 2)
                    {
                        this.x -= 3;
                    }
                    else if (this.radius == 3)
                    {
                        this.x += 3;
                    }

                    if (is_hit (field))
                    {
                        // 第3段階
                        if (this.radius == 0)
                        {
                            this.x += 3;
                            this.y += 1;
                        }
                        else if (this.radius == 1)
                        {
                            this.x += 3;
                            this.y -= 2;
                        }
                        else if (this.radius == 2)
                        {
                            -- this.y;
                        }
                        else if (this.radius == 3)
                        {
                            this.x -= 3;
                            this.y += 2;
                        }

                        if (is_hit (field))
                        {
                            // 第4段階
                            if (this.radius == 0)
                            {
                                this.x -= 3;
                                this.y -= 3;
                            }
                            else if (this.radius == 1)
                            {
                                this.x -= 3;
                                this.y += 3;
                            }
                            else if (this.radius == 2)
                            {
                                this.x += 3;
                                this.y += 3;
                            }
                            else if (this.radius == 3)
                            {
                                this.x += 3;
                                this.y -= 3;
                            }

                            if (is_hit (field))
                            {
                                // 失敗 => 元に戻す
                                
                                // 場所を元に戻す
                                this.x = backup_x;
                                this.y = backup_y;
                                
                                // 回転も元に戻す
                                rotateRight_force ();

                                Debug.Log ("回転失敗");
                                return false;
                            }
                        }
                    }
                }
            }

            // SRS成功後
            // 位置更新
            updatePos ();
        }
        
        return true;
    }

    // 右回転
    public bool rotateRight (Field field)
    {
        rotateRight_force ();

        // Super Rotation System
        if (is_hit (field))
        {
            // 補正前の座標を記憶
            int backup_x = this.x;
            int backup_y = this.y;

            // JSLTZミノについて
            // Oミノは必ず回転に成功するので考慮しない
            if (minotype != 'I')
            {
                // 第1段階
                if (this.radius == 0)
                {
                    -- this.x;
                }
                else if (this.radius == 1)
                {
                    -- this.x;
                }
                else if (this.radius == 2)
                {
                    ++ this.x;
                }
                else if (this.radius == 3)
                {
                    ++ this.x;
                }

                if (is_hit (field))
                {
                    // 第2段階
                    if (this.radius == 0 || this.radius == 2)
                    {
                        -- this.y;
                    }
                    else if (this.radius == 1 || this.radius == 3)
                    {
                        ++ this.y;
                    }

                    if (is_hit (field))
                    {
                        // 第3段階
                        this.x = backup_x;
                        this.y = backup_y;

                        if (this.radius == 0 || this.radius == 2)
                        {
                            this.y += 2;
                        }
                        else if (this.radius == 1 || this.radius == 3)
                        {
                            this.y -= 2;
                        }

                        if (is_hit (field))
                        {
                            // 第4段階
                            if (this.radius == 0)
                            {
                                -- this.x;
                            }
                            else if (this.radius == 1)
                            {
                                -- this.x;
                            }
                            else if (this.radius == 2)
                            {
                                ++ this.x;
                            }
                            else if (this.radius == 3)
                            {
                                ++ this.x;
                            }

                            if (is_hit (field))
                            {
                                // 失敗 => 元に戻す
                                
                                // 場所を元に戻す
                                this.x = backup_x;
                                this.y = backup_y;
                                
                                // 回転も元に戻す
                                rotateRight_force ();

                                Debug.Log ("回転失敗");
                                return false;
                            }
                        }
                    }
                }
            }
            // Iミノ
            else if (minotype == 'I')
            {
                // 第1段階
                if (this.radius == 0)
                {
                    this.x -= 2;
                }
                else if (this.radius == 1)
                {
                    this.x -= 2;
                }
                else if (this.radius == 2)
                {
                    -- this.x;
                }
                else if (this.radius == 3)
                {
                    this.x += 2;
                }

                if (is_hit (field))
                {
                    // 第2段階
                    if (this.radius == 0)
                    {
                        this.x += 3;
                    }
                    else if (this.radius == 1)
                    {
                        this.x += 3;
                    }
                    else if (this.radius == 2)
                    {
                        this.x += 3;
                    }
                    else if (this.radius == 3)
                    {
                        this.x -= 3;
                    }

                    if (is_hit (field))
                    {
                        // 第3段階
                        if (this.radius == 0)
                        {
                            this.y -= 2;
                        }
                        else if (this.radius == 1)
                        {
                            this.x -= 3;
                            this.y -= 1;
                        }
                        else if (this.radius == 2)
                        {
                            this.x -= 3;
                            this.y += 2;
                        }
                        else if (this.radius == 3)
                        {
                            this.x += 3;
                            this.y += 1;
                        }

                        if (is_hit (field))
                        {
                            // 第4段階
                            if (this.radius == 0)
                            {
                                this.x -= 3;
                                this.y += 3;
                            }
                            else if (this.radius == 1)
                            {
                                this.x += 3;
                                this.y += 3;
                            }
                            else if (this.radius == 2)
                            {
                                this.x += 3;
                                this.y -= 3;
                            }
                            else if (this.radius == 3)
                            {
                                this.x -= 3;
                                this.y -= 3;
                            }

                            if (is_hit (field))
                            {
                                // 失敗 => 元に戻す
                                
                                // 場所を元に戻す
                                this.x = backup_x;
                                this.y = backup_y;
                                
                                // 回転も元に戻す
                                rotateLeft_force ();

                                Debug.Log ("回転失敗");
                                return false;
                            }
                        }
                    }
                }
            }

            // SRS成功後
            // 位置更新
            updatePos ();
        }
        
        return true;
    }

    // Hold用にパラメーターを初期化する
    public void reset (int x, int y)
    {
        this.x = x;
        this.y = y;
        while (this.radius != 0)
        {
            rotateRight_force ();
        }
        updatePos ();
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
