
public static class Helper
{
    public static string FormatTime(float time)
    {
        int minutes = (int)(time / 60);
        int seconds = (int)(time % 60);
        int milliseconds = (int)((time - (minutes * 60) - seconds) * 1000);

        return string.Format("{0:00}:{1:00}", minutes, seconds, milliseconds);
    }
}
