Dexcom dex = new();

while (true)
{
    Console.Clear();
    string result = await dex.Run();
    int checkEveryXMinutes = 5;
    for (int i = 0; i < checkEveryXMinutes; i++)
    {
        Console.Clear();
        Console.WriteLine($"{result}{Environment.NewLine}Next report in {checkEveryXMinutes - i} { (i < checkEveryXMinutes - 1 ? "minutes" : "minute") }...");
        await Task.Delay(60000);
    }
}
