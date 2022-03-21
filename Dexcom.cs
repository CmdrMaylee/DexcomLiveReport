class Dexcom
{
    DexcomAPIComs apiComs = new();
    public string SessionId { get; set; } = string.Empty;

    public async Task<string> Run()
    {
        await SetSessionId();
        string completeJsonResult = await apiComs.GetLatestReadingAsJson(this);
        DexcomResponse readingResult = TranslateDexcomJson(completeJsonResult);
        return readingResult.ReturnInfo();
    }

    public async Task SetSessionId()
    {
        SessionId = await apiComs.LoginAndReturnSessionId();
    }

    public DexcomResponse TranslateDexcomJson(string json) // Turn http-responses into json data()
    {
        DexcomResponse reading = System.Text.Json.JsonSerializer.Deserialize<DexcomResponse>(json);
        return reading;
    }
}

class DexcomResponse
{
    public string WT { get; set; } = "";
    public string ST { get; set; } = "";
    public string DT { get; set; } = "";
    public double Value { get; set; } = 0D;
    public double MmolValue { get; set; } = 0D;
    public string Trend { get; set; } = "";

    public DexcomResponse(string WT, string ST, string DT, double Value, string Trend)
    {
        this.WT = WT;
        this.ST = ST;
        this.DT = DT;
        this.Value = Value;
        MmolValue = Value / 18;
        this.Trend = Trend;
    }

    public string ReturnInfo()
    {
        string result = "";
        result += MmolValue.ToString("N1") + " mmol";
        if (Trend == "NotComputable") Console.Write("?");
        if (Trend == "DoubleUp") result += "\u2191\u2191";
        if (Trend == "SingleUp") result += '\u2191';
        if (Trend == "FortyFiveUp") result += '\u2197';
        if (Trend == "Flat") result += '\u2192';
        if (Trend == "FortyFiveDown") result += '\u2198';
        if (Trend == "SingleDown") result += '\u2193';
        if (Trend == "DoubleDown") result += "\u2193\u2193";

        return result;
    }
}

class DexcomConstants // Constant values. Most of these have been gathered from various Github-repos, and were most likely dug up from the official Dexcom Share API itself.
{
    public static string currentlyUsedBaseUrl = baseNonUSUrl;
    public static string baseNonUSUrl = "https://shareous1.dexcom.com/ShareWebServices/Services/";
    public static string baseUSUrl = "https://share2.dexcom.com/ShareWebServices/Services/";
    public static string authenticateUserUriEndpoint = "General/AuthenticatePublisherAccount";
    public static string loginUriEndpoint = "General/LoginPublisherAccountById";
    public static string glucoseReadingsUriEndpoint = "Publisher/ReadPublisherLatestGlucoseValues";
    public static string appId = "d89443d2-327c-4a6f-89e5-496bbb0317db";
    public static string contentType = "application/json";
}
