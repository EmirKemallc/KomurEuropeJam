using UnityEngine;

public class Piece
{
    public Vector2Int[] cells;   // pivota göre hücreler
    public Vector2Int pos;       // pivotun gridde yeri
    public Color color;
    public int type;
    public int rotation;         // 0-3: saat yönünde kaç kez döndü

    public Piece(Vector2Int[] cells, Color color, int type)
    {
        this.cells = cells;
        this.color = color;
        this.type = type;
    }

    public Vector2Int WorldCell(int i)
    {
        return pos + cells[i];
    }

    // dir = 1: saat yönü, dir = -1: tersi
    public void Rotate(int dir)
    {
        if (type == 1) return;   // O parçası dönmez

        for (int i = 0; i < cells.Length; i++)
        {
            Vector2Int c = cells[i];
            cells[i] = dir > 0 ? new Vector2Int(c.y, -c.x) : new Vector2Int(-c.y, c.x);
        }
        rotation = (rotation + dir + 4) % 4;
    }
}