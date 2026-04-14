using UnityEngine;

public class TypeChart
{
    static float[][] chart =
    {  //                   NOR     WAT     FIR     GRS     ELE     EIS
       /*NOR*/ new float[]{ 1f,     1f,     1f,     1f,     1f,     1f},
       /*WAT*/ new float[]{ 1f,     0.5f,   2f,     0.5f,   0.5f,   1f},
       /*FIR*/ new float[]{ 1f,     0.5f,   0.5f,   2f,     1f,     2f},
       /*GRS*/ new float[]{ 1f,     2f,     0.5f,   0.5f,   1f,     1f},
       /*ELE*/ new float[]{ 1f,     2f,     1f,     0.5f,   0.5f,   0.5f},
       /*EIS*/ new float[]{ 1f,     0.5f,   0.5f,   2f,     1f,     0.5f}
    };

    public static float GetEffectiveness(ArigamiType attackType, ArigamiType defenstype)
    {
        if (attackType == ArigamiType.None || defenstype == ArigamiType.None)
        {
            return 1f;
        }

        int row = (int)attackType - 1;
        int col = (int)defenstype - 1;

        return chart[row][col];
    }
}
