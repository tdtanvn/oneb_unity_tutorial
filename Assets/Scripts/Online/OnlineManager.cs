using UnityEngine;
using OneB;
using System.Collections;
using System;
using Google.Protobuf.Collections;
using UnityEngine.Events;
using Random = System.Random;
using System.Linq;

public class OnlineManager : MonoBehaviour
{
    static OnlineManager s_Instance;
    public static OnlineManager Instance => s_Instance;
    public string GameId;
    public GameEnvironment Environment;
    public string GameVersion;

    [SerializeField] string m_SaveFilename = "demo.dat";

    struct Auth
    {
        public string playerId;
        public string secretKey;
    };
    private Auth auth;
    public OneBServicesClient API
    {
        get;
        private set;
    }
    public UnityEvent<PlayerProfile> OnPlayerProfile;
    void OnEnable()
    {
        SetupInstance();
    }
    async void SetupInstance()
    {
        if (s_Instance != null && s_Instance != this)
        {
            return;
        }
        API = new OneBServicesClient { GameId = GameId, Environment = Environment, GameVersion = GameVersion };
        s_Instance = this;
        Auth auth = GetOrCreateAuth();
        // _ = await Instance.API.Login(playerId: auth.playerId, secretKey: auth.secretKey);
        _ = await Instance.API.Send<LoginOutput>(new CallGameScriptCommand<LoginInput>("SocialAccount","Login",new LoginInput() {
            Provider = "google",
            IdToken = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ijk2OTcxODA4Nzk2ODI5YTk3MmU3OWE5ZDFhOWZmZjExY2Q2MWIxZTMiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2FjY291bnRzLmdvb2dsZS5jb20iLCJhenAiOiI4OTc3Mzk2MTcxOC1mdjQ5M2JobmExM3Uxbm02dmszYjg0MWU1bGlla3Uzbi5hcHBzLmdvb2dsZXVzZXJjb250ZW50LmNvbSIsImF1ZCI6Ijg5NzczOTYxNzE4LWZ2NDkzYmhuYTEzdTFubTZ2azNiODQxZTVsaWVrdTNuLmFwcHMuZ29vZ2xldXNlcmNvbnRlbnQuY29tIiwic3ViIjoiMTE2NDA3NjE3MDQ3NDAzMDU5NjI5IiwiYXRfaGFzaCI6ImFUZm5KM0wtbk81ODlHVVZSZXQ5SmciLCJuYW1lIjoiVHUgRGluaCBUYW4iLCJwaWN0dXJlIjoiaHR0cHM6Ly9saDMuZ29vZ2xldXNlcmNvbnRlbnQuY29tL2EvQUdObXl4YnVmY2VITTR5UHJVUnVRejZJRHNRLU1EelZSQXZGM0pRcjU5elBkdz1zOTYtYyIsImdpdmVuX25hbWUiOiJUdSBEaW5oIiwiZmFtaWx5X25hbWUiOiJUYW4iLCJsb2NhbGUiOiJlbiIsImlhdCI6MTY4MTk4NTk3MCwiZXhwIjoxNjgxOTg5NTcwfQ.ZszP3Di3ByHeEzj-gIjwpZGxLIr_CzF8dnq55hxmwLsK6weaRxz1fzsXyBjw6qIWYxdxaLtE4g1nfDFSZGAqZhX9AKhcbdRQeykQcD8R6t9bl6QmWMnUtzljHuiWWG-6BkRg011XT9Th1-1s8pj0KpfGtQPTveM_yA0OWc9AD7FiIKjf6YZu8CHT3WOWGeLW36zKKHr3xRbgGvaoWmyQQZx6JlMkurZOMULy0WMac3xbm8iNlUC7ijQdvJshz19PwvJz-xKyjDVVSK8v-rRkqTcYRjOAWPBIwnDYgIQOYknfQaiBml6aCsPUiIXk3iHHlgJBqeaApzAqhRIyBAKI3Q"
        }));
        GetPlayerProfile();
    }
    public static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
          .Select(s => s[random.Next(s.Length)]).ToArray());
    }
    private Auth GetOrCreateAuth()
    {
        if (!FileManager.LoadFromFile(m_SaveFilename, out var jsonString))
        {
            Auth auth;
            auth.playerId = GenerateRandomString(10);
            auth.secretKey = GenerateRandomString(10);
            FileManager.WriteToFile(m_SaveFilename, JsonUtility.ToJson(auth));
            return auth;
        }
        return JsonUtility.FromJson<Auth>(jsonString);
    }
    public async void GetPlayerProfile()
    {
        var result = await Instance.API.Send<PlayerProfile>(new GetPlayerObjectCommand("Profile"));
        OnPlayerProfile?.Invoke(result);
    }
    public IEnumerator GetInventory(Action<RepeatedField<PlayerData.Types.Inventory>> onCallback)
    {
        var playerData = Instance.API.Send<PlayerData>(new GetPlayerObjectCommand("Data"));
        yield return new WaitUntil(predicate: () => playerData.IsCompleted);
        if (playerData.Result?.Inventory != null)
        {
            onCallback.Invoke(playerData.Result.Inventory);
        }
    }
    public IEnumerator GetDailyRewards(Action<DailyRewardsList> onCallback)
    {
        var blueprintData = Instance.API.Send<DailyRewardsList>(new CallGameScriptCommand("DailyRewards", "GetList"));
        yield return new WaitUntil(predicate: () => blueprintData.IsCompleted);
        if (blueprintData.Result != null)
        {
            onCallback.Invoke(blueprintData.Result);
        }
    }
    public IEnumerator ClaimDailyRewards(Action<DailyRewardsClaimRewardsOutput> onCallback)
    {
        var blueprintData = Instance.API.Send<DailyRewardsClaimRewardsOutput>(new CallGameScriptCommand("DailyRewards", "ClaimRewards"));
        yield return new WaitUntil(predicate: () => blueprintData.IsCompleted);
        if (blueprintData.Result != null)
        {
            onCallback.Invoke(blueprintData.Result);
        }
    }
}
