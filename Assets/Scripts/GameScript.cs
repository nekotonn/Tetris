using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    // テトリミノの元になるprefab
    [SerializeField]
    private GameObject tetriminoPrefab_;

    // ブロックを積むところ
    [SerializeField]
    private Field field;


    // 落下中のテトリミノ
    private Tetrimino fallingTetrimino;


    // 乱数系
    private System.Random random;


    // タイマー系[単位:秒]
    // ゲームプレイ時間
    private float gametime;
    // 落下時間
    private float fallingtime;
    // 設置時間
    private float placetime;


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

    // Start is called before the first frame update
    void Start()
    {
        // シード初期化
        random = new System.Random ();
        
        // タイマー初期化
        gametime = 0.0f;
        fallingtime = 1.0f;
        placetime = 1.0f;
        

        fallingTetrimino = generateTetrimino (4, 18, generateRandom ());
    }

    // Update is called once per frame
    void Update()
    {
        // タイマー系カウント
        gametime += Time.deltaTime;
        fallingtime -= Time.deltaTime;
        placetime -= Time.deltaTime;

        if (fallingtime <= 0.0f)
        {
            // initialize
            fallingtime = 1.0f;


            // 落下処理
            if (fallingTetrimino.canfall (field))
            {
                // 落下可能な場合は落下
                fallingTetrimino.fall ();
            }
            else
            {
                // 落下できない場合は設置
                fallingTetrimino.place (field);

                // いらなくなったテトリミノを破棄
                Destroy (fallingTetrimino.gameObject);

                // 次のテトリミノを召喚
                fallingTetrimino = generateTetrimino (4, 18, generateRandom ());
            }
        }
    }
}
