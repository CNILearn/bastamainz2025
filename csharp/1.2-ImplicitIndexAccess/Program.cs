
TimerRemaining countdown = new()
{
    Buffer =
    {
        [^1] = 0,
        [^2] = 1,
        [^3] = 2,
        [^4] = 3,
        [^5] = 4,
        [^6] = 5,
        [^7] = 6,
        [^8] = 7,
        [^9] = 8,
        [^10] = 9,
    }
};

for (int i = 0; i < countdown.Buffer.Length; i++)
{
    Console.WriteLine(countdown.Buffer[i]);
    await Task.Delay(500);
}

public class TimerRemaining
{
    public int[] Buffer { get; set; } = new int[10];
}
