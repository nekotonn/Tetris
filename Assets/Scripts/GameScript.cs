using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScript : MonoBehaviour
{
    // テトリミノの元になるprefab
    [SerializeField]
    private GameObject tetriminoPrefab_;


    // ブロックを積むところ
    [SerializeField]
    private Field field;

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
    private Text debug_radius_info;


    // テトリミノ系
    // 落下中のテトリミノ
    private Tetrimino fallingTetrimino;
    // ホールド中のテトリミノ
    private Tetrimino holdingTetrimino;
    // ネクスト
    //private Tetrimino[] next;


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


    // 移動制限・回転制限
    private int move_count;
    private int rotate_count;
    private int move_limit;
    private int rotate_limit;


    // ホールド可能フラグ
    private bool holdable;


    // 乱数系
    // 1～7の乱数を返す関数(テトリミノが7種類なので)
    private int generateRandom ()
    {
        return random.Next (0, 7) + 1;
    }
    
    // テトリミノの生成
    Tetrimino generateTetrimino (int x, int y, int color)
    {
        var tetrimino = Instantiate <GameObject> (tetriminoPrefab_);
        tetrimino.transform.SetParent (field.GetComponent <RectTransform> (), false);
        var res = tetrimino.GetComponent <Tetrimino> ();
        res.init (x, y, color);
        return res;
    }

    // テトリミノの召喚
    void summon_fallingTetrimino ()
    {
        fallingTetrimino = generateTetrimino (4, 18, generateRandom ());
        fallingtime = 0.0f;
        placetime = 0.0f;
        move_count = 0;
        rotate_count = 0;
        holdable = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        // シード初期化
        random = new System.Random ();
        
        // タイマー初期化
        gametime = 0.0f;

        default_fallingInterval = 1.0f;
        softdrop_fallingInterval = 0.1f;
        fallingtimeInterval = default_fallingInterval;

        placetimeInterval = 1.0f;


        move_limit = 14;
        rotate_limit = 14;
        

        summon_fallingTetrimino ();
    }

    // Update is called once per frame
    void Update()
    {
        /********************************
        * タイマーカウント処理
        ********************************/

        gametime += Time.deltaTime;
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
            }
        }
        // 右キー入力で右に移動
        if (Input.GetKeyDown (KeyCode.RightArrow))
        {
            if (fallingTetrimino.canMoveRight (field))
            {
                fallingTetrimino.moveRight ();
                placetime = 0.0f;
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
            }
        }
        // スペースキーでホールド
        if (Input.GetKeyDown (KeyCode.Space))
        {
            if (holdable)
            {
                // swap
                var tmp = holdingTetrimino;
                holdingTetrimino = fallingTetrimino;
                fallingTetrimino = tmp;

                // 親をholdにする
                holdingTetrimino.gameObject.transform.SetParent (hold, false);
                holdingTetrimino.reset (0, 0);

                if (fallingTetrimino != null)
                {
                    // 親をfieldにする
                    fallingTetrimino.gameObject.transform.SetParent (field.GetComponent <RectTransform> (), false);
                    fallingTetrimino.reset (4, 18);
                }
                else
                {
                    // ホールドしてなかった場合は新しく召喚
                    summon_fallingTetrimino ();
                }
                
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
                fallingTetrimino.place (field);

                // いらなくなったテトリミノを破棄
                Destroy (fallingTetrimino.gameObject);

                // 次のテトリミノを召喚
                summon_fallingTetrimino ();
            }
        }

        hold_text.color = holdable ? new Color (1.0f, 1.0f, 1.0f) : new Color (0.5f, 0.5f, 0.5f);

        /********************************
        * debug
        ********************************/

        debug_gametime_text.text = "gametime: " + gametime.ToString ();
        debug_radius_info.text = "radius: " + fallingTetrimino.getRadius ().ToString ();
    }
}
