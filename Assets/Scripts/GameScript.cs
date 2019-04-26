using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameScript : MonoBehaviour
{
    // ブロックを積むところ、表示範囲
    [SerializeField]
    private RectTransform field_;

    // テトリミノの元になるprefab
    [SerializeField]
    private GameObject tetriminoPrefab_;

    // 落下中のテトリミノ
    private Tetrimino fallingTetrimino;
    

    Tetrimino generateTetrimino (int x, int y, int color)
    {
        var tetrimino = Instantiate <GameObject> (tetriminoPrefab_);
        tetrimino.transform.SetParent (field_, false);
        var res = tetrimino.GetComponent <Tetrimino> ();
        res.init (x, y, color);
        return res;
    }

    // Start is called before the first frame update
    void Start()
    {
        fallingTetrimino = generateTetrimino (4, 18, 3);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
