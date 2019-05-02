using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Block : MonoBehaviour
{
    
    // 青色の画像
    [SerializeField]
    private Sprite blueSprite;

    // 水色の画像
    [SerializeField]
    private Sprite cyanSprite;

    // 緑色の画像
    [SerializeField]
    private Sprite greenSprite;

    // 橙色の画像
    [SerializeField]
    private Sprite orangeSprite;

    // 紫色の画像
    [SerializeField]
    private Sprite purpleSprite;

    // 赤色の画像
    [SerializeField]
    private Sprite redSprite;

    // 黄色の画像
    [SerializeField]
    private Sprite yellowSprite;
    
    private int x;
    private int y;
    private int color;

    // 位置を変更したあとに呼び出して更新するためのもの
    private void updatePos ()
    {
        var transform = this.GetComponent <RectTransform> ();

        // ブロックのサイズを取得
        var rect = transform.rect;

        // 更新
        transform.anchoredPosition = new Vector2(x * rect.width, y * rect.height);
    }

    private void updateColor ()
    {
        // initialize
        var spriteTable = new Sprite[] {null,blueSprite,cyanSprite,greenSprite,orangeSprite,purpleSprite,redSprite,yellowSprite};
        
        // 更新
        this.GetComponent <Image> ().sprite = spriteTable [color];
    }

    
    public void init (int x, int y, int color)
    {
        // パラメーター更新
        this.x = x;
        this.y = y;
        this.color = color;
        
        // 位置更新
        updatePos ();

        // 色更新
        updateColor ();
    }


    // ブロックが他のブロックと重なっているかどうか
    public bool is_hit (Field field, int offset_x, int offset_y)
    {
        var tmp_x = this.x + offset_x;
        var tmp_y = this.y + offset_y;
        if (tmp_x < 0 || tmp_y < 0 || tmp_x >= 10 || tmp_y >= 22)
        {
            return true;
        }
        return field.getblock (tmp_x, tmp_y) != null;
    }


    // 左回転
    public void rotateLeft ()
    {
        // 回転軸
        int axis_x = 1;
        int axis_y = 1;
        
        // 水色(Iミノ)または黄色(Oミノ)は回転軸が(1.5,1.5)なので特殊処理をする
        if (this.color == 2 || this.color == 7)
        {
            this.x *= 2;
            this.y *= 2;

            axis_x = 3;
            axis_y = 3;
        }

        // 回転計算
        int delta_x = this.x - axis_x;
        int delta_y = this.y - axis_y;

        this.x = - delta_y + axis_x;
        this.y =   delta_x + axis_y;

        // 水色(Iミノ)または黄色(Oミノ)は回転軸が(1.5,1.5)なので特殊処理をする
        if (this.color == 2 || this.color == 7)
        {
            this.x /= 2;
            this.y /= 2;
        }

        // 位置更新
        updatePos ();
    }

    // 右回転
    public void rotateRight ()
    {
        // 回転軸
        int axis_x = 1;
        int axis_y = 1;
        
        // 水色(Iミノ)または黄色(Oミノ)は回転軸が(1.5,1.5)なので特殊処理をする
        if (this.color == 2 || this.color == 7)
        {
            this.x *= 2;
            this.y *= 2;

            axis_x = 3;
            axis_y = 3;
        }

        // 回転計算
        int delta_x = this.x - axis_x;
        int delta_y = this.y - axis_y;

        this.x =   delta_y + axis_x;
        this.y = - delta_x + axis_y;

        // 水色(Iミノ)または黄色(Oミノ)は回転軸が(1.5,1.5)なので特殊処理をする
        if (this.color == 2 || this.color == 7)
        {
            this.x /= 2;
            this.y /= 2;
        }

        // 位置更新
        updatePos ();
    }


    // 設置
    public void place (Field field, int offset_x, int offset_y)
    {
        // テトリミノからフィールドに移動
        this.gameObject.transform.SetParent (field.GetComponent <RectTransform> (), false);

        // テトリミノの位置分移動
        this.x += offset_x;
        this.y += offset_y;
        
        // 位置更新
        updatePos ();

        field.setblock (this.x, this.y, this);
    }

    // ライン消去用
    // 下に移動
    public void moveDown ()
    {
        -- this.y;
        updatePos ();
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
