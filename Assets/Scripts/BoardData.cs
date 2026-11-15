using UnityEngine;

public class BoardData
{
    private bool[,] occupied = new bool[BoardView.W, BoardView.H];
    private Color[,] colors = new Color[BoardView.W, BoardView.H];
    private int[,] types = new int[BoardView.W, BoardView.H];
    private int[,] cellIdx = new int[BoardView.W, BoardView.H];   // parçanın kaçıncı hücresi (0-3)
    private int[,] rots = new int[BoardView.W, BoardView.H];      // parçanın dönüş durumu (0-3)

    public bool IsOccupied(int x, int y) { return occupied[x, y]; }
    public Color GetColor(int x, int y) { return colors[x, y]; }
    public int GetPieceType(int x, int y) { return types[x, y]; }
    public int GetCellIndex(int x, int y) { return cellIdx[x, y]; }
    public int GetRotation(int x, int y) { return rots[x, y]; }

    public bool Fits(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int c = piece.WorldCell(i);
            if (c.x < 0 || c.x >= BoardView.W || c.y < 0) return false;
            if (c.y < BoardView.H && occupied[c.x, c.y]) return false;
        }
        return true;
    }

    public void Lock(Piece piece)
    {
        for (int i = 0; i < piece.cells.Length; i++)
        {
            Vector2Int c = piece.WorldCell(i);
            if (c.y >= BoardView.H) continue;
            occupied[c.x, c.y] = true;
            colors[c.x, c.y] = piece.color;
            types[c.x, c.y] = piece.type;
            cellIdx[c.x, c.y] = i;
            rots[c.x, c.y] = piece.rotation;
        }
    }

    public void Clear()
    {
        System.Array.Clear(occupied, 0, occupied.Length);
    }

    public int ClearFullLines()
    {
        int cleared = 0;

        for (int y = 0; y < BoardView.H; y++)
        {
            if (!IsRowFull(y)) continue;

            for (int yy = y; yy < BoardView.H - 1; yy++)
            {
                for (int x = 0; x < BoardView.W; x++)
                {
                    occupied[x, yy] = occupied[x, yy + 1];
                    colors[x, yy] = colors[x, yy + 1];
                    types[x, yy] = types[x, yy + 1];
                    cellIdx[x, yy] = cellIdx[x, yy + 1];
                    rots[x, yy] = rots[x, yy + 1];
                }
            }
            for (int x = 0; x < BoardView.W; x++)
                occupied[x, BoardView.H - 1] = false;

            cleared++;
            y--;
        }

        return cleared;
    }

    bool IsRowFull(int y)
    {
        for (int x = 0; x < BoardView.W; x++)
            if (!occupied[x, y]) return false;
        return true;
    }
}