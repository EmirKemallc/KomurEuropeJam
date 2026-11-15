public class ScoreKeeper
{
    public int score;
    public int lines;
    public int level;

    static readonly int[] LinePoints = { 0, 100, 300, 500, 800 };

    public void AddLines(int count)
    {
        if (count <= 0) return;
        score += LinePoints[count > 4 ? 4 : count] * (level + 1);
        lines += count;
        level = lines / 10;   // her 10 satırda seviye artar
    }

    public void Reset()
    {
        score = 0; lines = 0; level = 0;
    }
}