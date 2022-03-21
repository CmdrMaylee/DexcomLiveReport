using System.Text;

class DexcomAPIComs
{
    private HttpClient client = new();
    private string Username { get; set; } = ""; // Username goes here
    private string Password { get; set; } = ""; // Password goes here



    public async Task<string> PerformServerRequestAndReturnJson(Dictionary<string, string> sendData, bool authOrLogin = false, bool readGlucose = false, string uri = "")
    {
        string jsonBody = System.Text.Json.JsonSerializer.Serialize(sendData);
        StringContent content = new(jsonBody, Encoding.UTF8, "application/json");
        HttpResponseMessage response = await client.PostAsync(DexcomConstants.currentlyUsedBaseUrl + uri, content);
        string resultJson = await response.Content.ReadAsStringAsync();
        if (authOrLogin) resultJson = resultJson.Trim('"'); // The authentication token comes wrapped in double-quotes, and as such can't be sent in a request unless omitted.
        if (readGlucose) resultJson = resultJson.Replace("[", string.Empty).Replace("]", string.Empty); // The server always returns an array of dexcom readings, even if only one instance was returned. This removes the square-brackets and makes sure the one item returned isn't an array of one glucose reading.
        return resultJson;
    }

    public async Task<string> Authenticate() // Authentication involves logging in with user-credentials and getting the account-id, which will be used to get a session started.
    {
        Dictionary<string, string> authData = new()
        {
            { "applicationId", DexcomConstants.appId },
            { "accountName", Username },
            { "password", Password }
        };

        string jsonResponse = await PerformServerRequestAndReturnJson(authData, authOrLogin: true, uri: DexcomConstants.authenticateUserUriEndpoint);
        return jsonResponse;
    }

    public async Task<string> Login(string authCode) // The account-id from the above method is used here, and will return a session-id. This is used to get values tied to your Dexcom account.
    {
        Dictionary<string, string> loginData = new()
        {
            { "applicationId", DexcomConstants.appId },
            { "accountId", authCode },
            { "password", Password }
        };

        string jsonResponse = await PerformServerRequestAndReturnJson(loginData, authOrLogin: true, uri: DexcomConstants.loginUriEndpoint);
        return jsonResponse;
    }

    public async Task<string> ReadCurrentGlucoseData(Dexcom dex) // The Session-id from the above method is used here, and will return an array of dexcom readings. There's 1 reading every 10 minutes, hence why I'm defaulting the returned dexcom variables to only 1 over the span of 10 minutes.
    {
        Dictionary<string, string> fetchLatestValuesData = new()
        {
            { "sessionId", dex.SessionId },
            { "minutes", "10" },
            { "maxCount", "1" }
        };

        string jsonResponse = await PerformServerRequestAndReturnJson(fetchLatestValuesData, readGlucose: true, uri: DexcomConstants.glucoseReadingsUriEndpoint);
        return jsonResponse;
    }

    public async Task<string> LoginAndReturnSessionId()
    {
        string userId = await Authenticate();
        string sessionId = await Login(userId);
        return sessionId;
    }

    public async Task<string> GetLatestReadingAsJson(Dexcom dex) // Automatically perform the fetch-step
    {
        if (String.IsNullOrEmpty(dex.SessionId))
        {
            string accountId = await Authenticate();
            dex.SessionId = await Login(accountId);
        }
        string fetchdValues = "";
        fetchdValues = await ReadCurrentGlucoseData(dex);
        fetchdValues = fetchdValues.Replace("[", string.Empty).Replace("]", string.Empty);
        DexcomResponse currentDexcomReading = System.Text.Json.JsonSerializer.Deserialize<DexcomResponse>(fetchdValues);
        fetchdValues = await ReadCurrentGlucoseData(dex);

        return fetchdValues;
    }
}
