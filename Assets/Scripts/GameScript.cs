using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class GameScript : MonoBehaviour
{
    // テトリミノの元になるprefab
    [SerializeField]
    private GameObject tetriminoPrefab_;


    // ブロックを積むところ
    [SerializeField]
    private Field field;

    // ネクスト管理
    [SerializeField]
    private RectTransform next;

    // ホールド表示場所
    [SerializeField]
    private RectTransform hold;
    
    // HOLDって文字
    [SerializeField]
    private Text hold_text;

    
    // デバッグ情報表示
    [SerializeField]
    private Text debug_gametime_text;

    [SerializeField]
    private Text debug_angle_info;
    
    [SerializeField]
    private Text ren_text;

    [SerializeField]
    private Text mini_text;
    
    [SerializeField]
    private Text tetris_text;

    [SerializeField]
    private Text back_to_back_text;
    
    [SerializeField]
    private Text gameover_text;


    // 定数系
    // 召喚位置
    private const int summon_x = 3;
    private const int summon_y = 17;


    // テトリミノ系
    // 落下中のテトリミノ
    private Tetrimino fallingTetrimino;
    // ネクスト
    private List <Tetrimino> nextTetriminos = new List <Tetrimino> ();
    // ホールド中のテトリミノ
    private Tetrimino holdingTetrimino;


    // 乱数系
    private System.Random random;


    // タイマー系[単位:秒]
    // ゲームプレイ時間
    private float gametime;

    // 落下時間
    private float fallingtime;
    private float default_fallingInterval;
    private float softdrop_fallingInterval;
    private float fallingtimeInterval;

    // 設置時間
    private float placetime;
    private float placetimeInterval;

    // 
    private float show_special_move_time;
    private float show_special_move_time_interval;


    // 移動制限・回転制限
    private int move_count;
    private int rotate_count;
    private int move_limit;
    private int rotate_limit;


    // フラグ系
    // ホールド可能フラグ
    private bool holdable;
    // 着地前に回転したか(T-Spin判定用)
    private bool last_rotated;

    // 継続フラグ
    // Tetris
    private string tetris;
    // T-Spinフラグ
    private bool T_Spin;
    // T-Spin Mini
    private bool mini;
    // Back-to-Back
    private bool back_to_back_continue_flag;
    private bool back_to_back;
    // REN
    private int ren;
    // Perfect Clear
    private bool perfect_clear;

    private bool pause;
    private bool gameover;

    
    // テトリミノの生成
    Tetrimino generateTetrimino (int color)
    {
        var tetrimino = Instantiate <GameObject> (tetriminoPrefab_);
        tetrimino.transform.SetParent (next, false);
        var res = tetrimino.GetComponent <Tetrimino> ();
        res.init (0, 0, color);
        return res;
    }

    // ネクスト補充
    void replenish_next ()
    {
        // generate
        var minos = new Tetrimino[] {null,null,null,null,null,null,null};
        for (int i = 0; i < minos.Length; ++ i)
        {
            minos [i] = generateTetrimino (i + 1);
        }

        // shuffle
        // Fisher-Yatesアルゴリズム
        for (int i = minos.Length - 1; i > 0; -- i)
        {
            int k = random.Next (i + 1);
            // swap
            var tmp = minos [k];
            minos [k] = minos [i];
            minos [i] = tmp;
        }

        // add
        nextTetriminos.AddRange (minos);
    }

    // テトリミノの召喚
    void summon_fallingTetrimino ()
    {
        // ネクストから取り出す
        fallingTetrimino = nextTetriminos [0];
        nextTetriminos.RemoveAt (0);

        // 位置初期化
        fallingTetrimino.reset (field.GetComponent <RectTransform> (), summon_x, summon_y);

        // なんかいろいろ初期化
        fallingtime = 0.0f;
        placetime = 0.0f;
        move_count = 0;
        rotate_count = 0;
        holdable = true;
        last_rotated = false;

        // ネクストが7個以下になったら補充する
        if (nextTetriminos.Count <= 7)
        {
            replenish_next ();
        }

        // 召喚時に重なっていたらGAME OVER
        gameover = fallingTetrimino.is_hit (field);
    }

    private IEnumerator delay_method (float waittime, Action action)
    {
        yield return new WaitForSeconds (waittime);
        action ();
    }

    // waittime[sec]後にactionを実行する
    void schedule (float waittime , Action action)
    {
        StartCoroutine (delay_method (waittime , action));
    }

    // Start is called before the first frame update
    void Start()
    {
        // シード初期化
        random = new System.Random ();

        // ネクスト補充
        replenish_next ();
        
        // タイマー初期化
        gametime = 0.0f;

        default_fallingInterval = 1.0f;
        softdrop_fallingInterval = 0.05f;
        fallingtimeInterval = default_fallingInterval;

        placetimeInterval = 1.0f;

        show_special_move_time = 3.0f;
        show_special_move_time_interval = 3.0f;


        move_limit = 14;
        rotate_limit = 14;


        // フラグ初期化
        tetris = "";
        T_Spin = false;
        mini = false;
        back_to_back_continue_flag = false;
        back_to_back = false;
        ren = -1;
        perfect_clear = false;

        pause = false;
        gameover = false;
        

        summon_fallingTetrimino ();
    }

    // Update is called once per frame
    void Update()
    {
        /********************************
        * タイマーカウント処理
        ********************************/

        if (! gameover)
        {
            gametime += Time.deltaTime;
        }
        show_special_move_time += Time.deltaTime;

        /********************************
        * ゲームオーバー時処理
        ********************************/

        if (gameover)
        {
            if (Input.GetKeyDown (KeyCode.Return))
            {
                Application.LoadLevel (0);
            }
        }

        if (! pause && ! gameover)
        {
            fallingtime += Time.deltaTime;
            placetime += Time.deltaTime;

            /********************************
            * キー入力処理
            ********************************/

            // 上キーでハードドロップ
            if (Input.GetKeyDown (KeyCode.UpArrow))
            {
                // TODO: ハードドロップ処理
                while (fallingTetrimino.canFall (field))
                {
                    fallingTetrimino.fall ();
                }
                placetime = placetimeInterval;
                last_rotated = false;
            }
            // 下キーでソフトドロップ
            if (Input.GetKeyDown (KeyCode.DownArrow))
            {
                fallingtime = fallingtimeInterval = softdrop_fallingInterval;
            }
            if (Input.GetKeyUp (KeyCode.DownArrow))
            {
                fallingtime = 0.0f;
                fallingtimeInterval = default_fallingInterval;
            }
            // 左キー入力で左に移動
            if (Input.GetKeyDown (KeyCode.LeftArrow))
            {
                if (fallingTetrimino.canMoveLeft (field))
                {
                    fallingTetrimino.moveLeft ();
                    placetime = 0.0f;
                    last_rotated = false;
                }
            }
            // 右キー入力で右に移動
            if (Input.GetKeyDown (KeyCode.RightArrow))
            {
                if (fallingTetrimino.canMoveRight (field))
                {
                    fallingTetrimino.moveRight ();
                    placetime = 0.0f;
                    last_rotated = false;
                }
            }
            // Zキーで左回転
            if (Input.GetKeyDown (KeyCode.Z))
            {
                // 回転に成功した場合
                if (fallingTetrimino.rotateLeft (field))
                {
                    // 設置時間リセット
                    placetime = 0.0f;
                    last_rotated = true;
                }
            }
            // Xキーで右回転
            if (Input.GetKeyDown (KeyCode.X))
            {
                // 回転に成功した場合
                if (fallingTetrimino.rotateRight (field))
                {
                    // 設置時間リセット
                    placetime = 0.0f;
                    last_rotated = true;
                }
            }
            // スペースキーでホールド
            if (Input.GetKeyDown (KeyCode.Space))
            {
                if (holdable)
                {
                    // ネクストの先頭にholdingを無理やり突っ込む
                    // こうすることで<s>めんどくさい</s>初期化処理をsummon_fallingTetrimino()に丸投げできる
                    if (holdingTetrimino != null)
                    {
                        nextTetriminos.Insert (0, holdingTetrimino);
                    }

                    // 回転リセット
                    fallingTetrimino.reset_rotation ();

                    // ホールド
                    holdingTetrimino = fallingTetrimino;

                    // 親をholdにする
                    holdingTetrimino.reset (hold, 0, 0);

                    // 召喚
                    summon_fallingTetrimino ();
                    
                    // initialize
                    holdable = false;
                }
            }


            /********************************
            * 時間経過処理
            ********************************/

            if (fallingtime >= fallingtimeInterval)
            {
                // initialize
                fallingtime = 0.0f;


                // 落下処理
                if (fallingTetrimino.canFall (field))
                {
                    // 落下可能な場合は落下
                    fallingTetrimino.fall ();

                    // 設置時間リセット
                    placetime = 0.0f;

                    last_rotated = false;
                }
                else
                {
                    // 設置処理は移動しました。
                }
            }


            if (placetime >= placetimeInterval)
            {
                // initialize
                placetime = 0.0f;

                // 落下できない場合
                if (! fallingTetrimino.canFall (field))
                {
                    // T-Spin判定
                    switch (last_rotated ? fallingTetrimino.check_T_Spin (field) : 0)
                    {
                        case 0:
                            T_Spin = false;
                            mini = false;
                            break;
                        case 1:
                            T_Spin = true;
                            mini = true;
                            break;
                        case 2:
                            T_Spin = true;
                            mini = false;
                            break;
                        default:
                            break;
                    }

                    // 設置
                    fallingTetrimino.place (field);

                    // いらなくなったテトリミノを破棄
                    Destroy (fallingTetrimino.gameObject);

                    // ライン消去
                    int line = field.clear_lines ();

                    // REN判定
                    ren = line > 0 ? ren + 1 : -1;

                    // Tetris判定
                    // Back-to-Back判定
                    if (line == 4)
                    {
                        tetris = "Tetris";
                        back_to_back = back_to_back_continue_flag;
                        back_to_back_continue_flag = true;
                        show_special_move_time = 0.0f;
                    }
                    else if (line == 3 && T_Spin)
                    {
                        tetris = "T-Spin\nTriple";
                        mini = false;
                        back_to_back = back_to_back_continue_flag;
                        back_to_back_continue_flag = true;
                        show_special_move_time = 0.0f;
                    }
                    else if (line == 2 && T_Spin)
                    {
                        tetris = "T-Spin\nDouble";
                        mini = false;
                        back_to_back = back_to_back_continue_flag;
                        back_to_back_continue_flag = true;
                        show_special_move_time = 0.0f;
                    }
                    else if (line == 1 && T_Spin)
                    {
                        tetris = "T-Spin\nSingle";
                        back_to_back = back_to_back_continue_flag;
                        back_to_back_continue_flag = true;
                        show_special_move_time = 0.0f;
                    }
                    else if (line > 0)
                    {
                        tetris = "";
                        back_to_back = false;
                        back_to_back_continue_flag = false;
                    }
                    else if (T_Spin)
                    {
                        tetris = "T-Spin\n";
                        back_to_back = false;
                        show_special_move_time = 0.0f;
                    }

                    // ラインを消した場合遅延させる
                    if (line > 0)
                    {
                        // 停止
                        pause = true;

                        schedule (0.5f , () => {
                            // 再開
                            pause = false;
                            
                            // フィールド修正
                            field.updatePos ();

                            // 次のテトリミノの出現も遅延させる
                            summon_fallingTetrimino ();
                        });

                        fallingTetrimino = null;
                    }
                    else
                    {
                        // 次のテトリミノを召喚
                        summon_fallingTetrimino ();
                    }
                }
            }
        
        }


        // テキスト描画
        hold_text.color = holdable ? new Color (1.0f, 1.0f, 1.0f) : new Color (0.5f, 0.5f, 0.5f);
        ren_text.text = ren > 0 ? ren.ToString () + "\nREN" : "";
        gameover_text.gameObject.SetActive (gameover);
        
        // テキスト描画その2
        if (show_special_move_time < show_special_move_time_interval)
        {
            mini_text.gameObject.SetActive (mini);
            tetris_text.text = tetris;
            back_to_back_text.gameObject.SetActive (back_to_back);
        }
        else
        {
            mini_text.gameObject.SetActive (false);
            tetris_text.text = "";
            back_to_back_text.gameObject.SetActive (false);
        }


        // ネクスト描画
        for (int i = 0; i < nextTetriminos.Count; ++ i)
        {
            nextTetriminos [i].setPos (0, (5 - i) * 4);
        }

        /********************************
        * debug
        ********************************/

        debug_gametime_text.text = "gametime: " + gametime.ToString ();
        debug_angle_info.text = fallingTetrimino == null ? "null" : "angle: " + fallingTetrimino.getAngle ().ToString ();
    }
}
