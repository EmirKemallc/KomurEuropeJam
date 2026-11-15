using UnityEngine;

[System.Serializable]
public class PieceSprites
{
    public Sprite[] cells = new Sprite[4];   // parçanın doğduğu hâline göre hücre sprite'ları
}

public class BoardView : MonoBehaviour
{
    public const int W = 10;
    public const int H = 20;

    // Kamera görüş alanı (dünya y değerleri): üstte skor + önizleme, altta tuşlar için boşluk
    public const float ViewTop = H + 4f;
    public const float ViewBottom = -6f;

    const int PW = 6, PH = 4;   // sıradaki parça önizleme alanı

    static readonly Color EmptyColor = new Color(0.98f, 0.93f, 0.92f);

    public PieceSprites[] pieceSprites;   // 7 eleman: 0=I 1=O 2=T 3=S 4=Z 5=J 6=L

    private SpriteRenderer[,] cells = new SpriteRenderer[W, H];
    private SpriteRenderer[,] previewCells = new SpriteRenderer[PW, PH];
    private Sprite squareSprite;

    public void Build()
    {
        squareSprite = MakeSquareSprite();
        SetupCamera();

        for (int x = 0; x < W; x++)
        {
            for (int y = 0; y < H; y++)
            {
                GameObject go = new GameObject("Cell " + x + "," + y);
                go.transform.SetParent(transform);
                go.transform.position = new Vector3(x, y, 0);

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                Paint(sr, EmptyColor, -1, 0, 0);
                cells[x, y] = sr;
            }
        }

        BuildPreview();
    }

    // Pencere boyutu ya da ekran yönü değişirse (tam ekran, telefonu çevirme) düzen uyum sağlasın
    void Update()
    {
        UpdateCamera();
    }

    public void Clear()
    {
        for (int x = 0; x < W; x++)
            for (int y = 0; y < H; y++)
                Paint(cells[x, y], EmptyColor, -1, 0, 0);
    }

    public void SetCell(int x, int y, Color color, int type, int cell, int rotation)
    {
        if (x < 0 || x >= W || y < 0 || y >= H) return;
        Paint(cells[x, y], color, type, cell, rotation);
    }

    public void ShowPreview(Piece piece)
    {
        for (int x = 0; x < PW; x++)
            for (int y = 0; y < PH; y++)
                Paint(previewCells[x, y], Color.clear, -1, 0, 0);

        // Parça şekli x:-1..2, y:-1..1 aralığında, alanın ortasına denk gelsin diye (2,1) kaydırıyoruz
        for (int i = 0; i < piece.cells.Length; i++)
        {
            int px = 2 + piece.cells[i].x;
            int py = 1 + piece.cells[i].y;
            Paint(previewCells[px, py], piece.color, piece.type, i, piece.rotation);
        }
    }

    // Sprite varsa hücreye sığdırır ve parçanın dönüşü kadar çevirir, yoksa renkli kareyi çizer
    void Paint(SpriteRenderer sr, Color color, int type, int cell, int rotation)
    {
        Sprite s = null;
        if (type >= 0 && pieceSprites != null && type < pieceSprites.Length
            && pieceSprites[type] != null && cell < pieceSprites[type].cells.Length)
            s = pieceSprites[type].cells[cell];

        if (s != null)
        {
            sr.sprite = s;
            sr.color = Color.white;
            sr.transform.localScale = new Vector3(1f / s.bounds.size.x, 1f / s.bounds.size.y, 1f);
            sr.transform.rotation = Quaternion.Euler(0f, 0f, -90f * rotation);
        }
        else
        {
            sr.sprite = squareSprite;
            sr.color = color;
            sr.transform.localScale = Vector3.one * 0.92f;
            sr.transform.rotation = Quaternion.identity;
        }
    }

    // Önizleme ızgaranın üstünde, sağ tarafa hizalı (x: 5..10, y: H+0.5..)
    void BuildPreview()
    {
        Vector3 origin = new Vector3(W - 5, H + 0.5f, 0);
        for (int x = 0; x < PW; x++)
        {
            for (int y = 0; y < PH; y++)
            {
                GameObject go = new GameObject("Preview " + x + "," + y);
                go.transform.SetParent(transform);
                go.transform.position = origin + new Vector3(x, y, 0);

                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                Paint(sr, Color.clear, -1, 0, 0);   // boşken görünmez
                previewCells[x, y] = sr;
            }
        }
    }

    void SetupCamera()
    {
        Camera cam = Camera.main;
        cam.orthographic = true;
        cam.backgroundColor = new Color(0.85f, 0.65f, 0.69f);   // toz pembe
        cam.clearFlags = CameraClearFlags.SolidColor;
        UpdateCamera();
    }

    // Izgara her zaman yatayda ortalı ve tam görünür, dikeyde skor/tuş alanı sığar
    void UpdateCamera()
    {
        Camera cam = Camera.main;
        float aspect = (float)Screen.width / Screen.height;

        float sizeByHeight = (ViewTop - ViewBottom) / 2f;
        float sizeByWidth = (W + 1f) / (2f * aspect);   // dar ekranda genişliğe göre büyüt

        cam.orthographicSize = Mathf.Max(sizeByHeight, sizeByWidth);
        cam.transform.position = new Vector3((W - 1) / 2f, (ViewTop + ViewBottom) / 2f, -10f);
    }

    Sprite MakeSquareSprite()
    {
        Texture2D tex = new Texture2D(4, 4);
        Color[] px = new Color[16];
        for (int i = 0; i < 16; i++) px[i] = Color.white;
        tex.SetPixels(px);
        tex.filterMode = FilterMode.Point;
        tex.Apply();
        return Sprite.Create(tex, new Rect(0, 0, 4, 4), new Vector2(0.5f, 0.5f), 4f);
    }
}