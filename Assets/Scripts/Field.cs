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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
