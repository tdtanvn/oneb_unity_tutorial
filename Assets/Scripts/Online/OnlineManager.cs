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
        _ = await Instance.API.Login(playerId: auth.playerId, secretKey: auth.secretKey);
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
