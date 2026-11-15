using System.Collections.Generic;
using UnityEngine;

public class PieceBag
{
    // Tür sırası: 0=I 1=O 2=T 3=S 4=Z 5=J 6=L (sprite'lar gelince bu sırayla bağlanacak)
    static readonly Vector2Int[][] Shapes =
    {
        new[]{ new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(2,0) },
        new[]{ new Vector2Int(0,0),  new Vector2Int(1,0), new Vector2Int(0,1), new Vector2Int(1,1) },
        new[]{ new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(1,0), new Vector2Int(0,-1) },
        new[]{ new Vector2Int(-1,0), new Vector2Int(0,0), new Vector2Int(0,1), new Vector2Int(1,1) },
        new[]{ new Vector2Int(-1,1), new Vector2Int(0,1), new Vector2Int(0,0), new Vector2Int(1,0) },
        new[]{ new Vector2Int(-1,1), new Vector2Int(-1,0),new Vector2Int(0,0), new Vector2Int(1,0) },
        new[]{ new Vector2Int(1,1),  new Vector2Int(-1,0),new Vector2Int(0,0), new Vector2Int(1,0) },
    };

    static readonly Color[] Colors =
    {
        new Color(0.2f, 0.9f, 0.9f), new Color(0.95f, 0.9f, 0.2f), new Color(0.7f, 0.3f, 0.9f),
        new Color(0.3f, 0.85f, 0.3f), new Color(0.9f, 0.25f, 0.25f), new Color(0.25f, 0.35f, 0.95f),
        new Color(0.95f, 0.6f, 0.2f),
    };

    private List<int> bag = new List<int>();
    private int next;

    public PieceBag()
    {
        next = Draw();
    }

    public int PeekType() { return next; }

    public Piece TakeNext()
    {
        int t = next;
        next = Draw();
        return Make(t);
    }

    public static Piece Make(int type)
    {
        return new Piece((Vector2Int[])Shapes[type].Clone(), Colors[type], type);
    }

    // 7-bag: her 7 parçada her tür bir kez gelir, uzun süre aynı parça gelmez
    int Draw()
    {
        if (bag.Count == 0)
        {
            for (int i = 0; i < 7; i++) bag.Add(i);
            for (int i = bag.Count - 1; i > 0; i--)
            {
                int j = Random.Range(0, i + 1);
                int tmp = bag[i]; bag[i] = bag[j]; bag[j] = tmp;
            }
        }
        int t = bag[bag.Count - 1];
        bag.RemoveAt(bag.Count - 1);
        return t;
    }
}