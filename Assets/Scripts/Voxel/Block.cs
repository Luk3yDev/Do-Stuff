using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Block
{
    int id;
    public Vector2 uvCoordinate { get; private set; }

    public static Block drygrass = new Block(1, 0, 0);
    public static Block dirt = new Block(2, 0, 1);
    public static Block greywacke = new Block(3, 0, 2);
    public static Block limestone = new Block(4, 0, 3);

    public Block (int identifier, int uvCoordX, int uvCoordY)
    {
        id = identifier;
        uvCoordinate = new Vector2(uvCoordX, uvCoordY);
    }

    public static int IDFromBlock(Block block)
    {
        return block.id;
    }

    public static Block BlockFromID(int id)
    {
        switch (id)
        {
            case 1:
                return drygrass;
            case 2:
                return dirt;
            case 3:
                return greywacke;
            case 4:
                return limestone;
            default:
                return null;
        }
    }
}
