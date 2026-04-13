using UnityEngine;
using UnityEngine.Tilemaps;

public enum WallType
{
    Open,       // Frei begehbar
    Solid,      // Diese Kante ist eine Wand (Stopp)
    Jumpable    // Diese Kante erfordert einen Sprung
}

[CreateAssetMenu(fileName = "RPG Tile", menuName = "RPG Tile")]
public class RPGTile : Tile
{

    [Header("Kollision")]
    public bool isFullBlock;
    public WallType wallTop;
    public WallType wallRight;
    public WallType wallLeft;
    public WallType wallBottom;

    [Header("Events")]
    public bool canHaveEncounter;

}
