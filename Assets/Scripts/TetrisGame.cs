using UnityEngine;

[RequireComponent(typeof(BoardView))]
public class TetrisGame : MonoBehaviour
{
    [Header("Düşme")]
    public float fallInterval = 0.5f;
    public float softDropInterval = 0.05f;

    [Header("Yan hareket")]
    public float moveDelay = 0.17f;
    public float moveRepeat = 0.05f;

    private BoardView board;
    private BoardData data = new BoardData();
    private ScoreKeeper stats = new ScoreKeeper();
    private PieceBag bag;   // Start'ta oluşturuluyor (Random alan tanımında çağrılamaz)
    private Piece current;
    private float fallTimer;
    private float moveTimer;
    private bool gameOver;
    private int highScore;
    private bool newRecord;

    void Start()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        board = GetComponent<BoardView>();
        board.Build();
        bag = new PieceBag();
        SpawnPiece();
    }

    void Update()
    {
        if (gameOver)
        {
            if (Input.GetKeyDown(KeyCode.R)) Restart();
            return;
        }

        HandleHorizontal();
        HandleRotate();
        HandleFall();
        Draw();
    }

    void Restart()
    {
        data.Clear();
        stats.Reset();
        bag = new PieceBag();
        gameOver = false;
        newRecord = false;
        SpawnPiece();
        Draw();
    }

    void HandleHorizontal()
    {
        int dir = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) dir = -1;
        else if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) dir = 1;

        if (dir == 0) { moveTimer = 0f; return; }

        bool justPressed = Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)
                        || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow);

        if (justPressed)
        {
            TryMoveSideways(dir);
            moveTimer = moveDelay;
            return;
        }

        moveTimer -= Time.deltaTime;
        if (moveTimer <= 0f)
        {
            TryMoveSideways(dir);
            moveTimer = moveRepeat;
        }
    }

    void TryMoveSideways(int dir)
    {
        current.pos.x += dir;
        if (!data.Fits(current))
            current.pos.x -= dir;
    }

    void HandleRotate()
    {
        if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
            TryRotate();
    }

    void TryRotate()
    {
        Vector2Int[] oldCells = (Vector2Int[])current.cells.Clone();
        Vector2Int oldPos = current.pos;
        int oldRot = current.rotation;

        current.Rotate(1);

        int[] kicks = { 0, -1, 1, -2, 2 };
        foreach (int k in kicks)
        {
            current.pos = new Vector2Int(oldPos.x + k, oldPos.y);
            if (data.Fits(current)) return;
        }

        // Sığmadı, her şeyi eski hâline döndür (dönüş sayısı dahil)
        current.cells = oldCells;
        current.pos = oldPos;
        current.rotation = oldRot;
    }

    void HandleFall()
    {
        bool soft = Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow);
        float levelInterval = Mathf.Max(0.1f, fallInterval * Mathf.Pow(0.85f, stats.level));
        float interval = soft ? softDropInterval : levelInterval;

        fallTimer += Time.deltaTime;
        if (fallTimer < interval) return;
        fallTimer = 0f;

        current.pos.y -= 1;
        if (!data.Fits(current))
        {
            current.pos.y += 1;
            data.Lock(current);
            stats.AddLines(data.ClearFullLines());
            SpawnPiece();
        }
    }

    void SpawnPiece()
    {
        current = bag.TakeNext();
        current.pos = new Vector2Int(BoardView.W / 2, BoardView.H - 2);
        fallTimer = 0f;

        board.ShowPreview(PieceBag.Make(bag.PeekType()));

        // Yeni parça doğar doğmaz yığına çarpıyorsa oyun biter
        if (!data.Fits(current))
        {
            gameOver = true;

            if (stats.score > highScore)
            {
                highScore = stats.score;
                newRecord = true;
                PlayerPrefs.SetInt("HighScore", highScore);
                PlayerPrefs.Save();
            }
        }
    }

    void Draw()
    {
        board.Clear();

        for (int x = 0; x < BoardView.W; x++)
            for (int y = 0; y < BoardView.H; y++)
                if (data.IsOccupied(x, y))
                    board.SetCell(x, y, data.GetColor(x, y), data.GetPieceType(x, y),
                                  data.GetCellIndex(x, y), data.GetRotation(x, y));

        for (int i = 0; i < current.cells.Length; i++)
        {
            Vector2Int c = current.WorldCell(i);
            board.SetCell(c.x, c.y, current.color, current.type, i, current.rotation);
        }
    }

    void OnGUI()
    {
        Camera cam = Camera.main;

        float leftEdge = cam.WorldToScreenPoint(new Vector3(-0.5f, 0f, 0f)).x;
        float rightX = cam.WorldToScreenPoint(new Vector3(BoardView.W + 0.5f, 0f, 0f)).x;
        float leftW = Mathf.Max(120f, leftEdge - 40f);

        int fs = Mathf.Max(14, Screen.height / 34);
        Color ink = new Color(0.15f, 0.12f, 0.1f);

        GUIStyle text = new GUIStyle(GUI.skin.label) { fontSize = fs, wordWrap = true };
        text.normal.textColor = ink;

        GUIStyle title = new GUIStyle(text) { fontSize = fs + 4, fontStyle = FontStyle.Bold };

        // Left: how to play
        float top = Screen.height * 0.15f;
        GUI.Label(new Rect(20, top, leftW, 40), "How to play", title);
        GUI.Label(new Rect(20, top + 44, leftW, 140),
            "A / D : Move\nW : Rotate\nS : Soft drop", text);

        // Left: game over + high score
        if (gameOver)
        {
            GUIStyle over = new GUIStyle(title) { fontSize = fs + 12 };
            over.normal.textColor = new Color(0.7f, 0.1f, 0.1f);

            float y = Screen.height * 0.5f;
            GUI.Label(new Rect(20, y, leftW, 60), "Game over", over);
            GUI.Label(new Rect(20, y + 60, leftW, 60), "Press R to try again", text);

            GUI.Label(new Rect(20, y + 110, leftW, 40), "High score: " + highScore, title);
            if (newRecord)
                GUI.Label(new Rect(20, y + 150, leftW, 40), "New record!", text);
        }

        // Right: score and level
        GUI.Label(new Rect(rightX, Screen.height * 0.45f, 300, 100),
            "Score: " + stats.score + "\nLevel: " + stats.level, title);
    }
}