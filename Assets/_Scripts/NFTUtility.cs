using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class NFTUtility : MonoBehaviour
{
    public class Response<T> { public T response; }
    [SerializeField] private UserNFTs dataContainer;
    private readonly static string host = "https://api.gaming.chainsafe.io/evm";
    public UnityEvent<string> OnUserWNFTsReceived;
    public UnityEvent<string> OnUserGOBotsReceived;

    public void LoadGoBots()
    {
        string chain = "polygon";
        string network = "mainnet";
        string account = PlayerPrefs.GetString("Account");
#if !UNITY_EDITOR
        if (string.IsNullOrEmpty(account)) return;
#endif
        string contract = "0x6AE43f0F31B2585a5a84c13E33355a204F5D7b46";
        int first = 200;
        int skip = 0;
        Load(chain, network, account, contract, first, skip, Collection.GOBots);
    }
    public void LoadWNFTs()
    {
        string chain = "polygon";
        string network = "mainnet";
        string account = PlayerPrefs.GetString("Account");
#if !UNITY_EDITOR
        Debug.Log(account);
        if (string.IsNullOrEmpty(account)) return;
#endif
        string contract = "0x86702dd104c328578cdc83ce69b306accf0b57ef";
        int first = 200;
        int skip = 0;
        Load(chain, network, account, contract, first, skip, Collection.WNFTs);
    }
    private void Load(string chain, string network, string account, string contract, int first, int skip, Collection clction)
    {
        StartCoroutine(AllErc721(chain, network, account, clction, contract, first, skip));
    }
    private IEnumerator AllErc721(string _chain, string _network, string _account, Collection clction, string _contract = "", int _first = 500, int _skip = 0)
    {
        Debug.Log($"Sending to host for {_account}, {clction}");
        WWWForm form = new WWWForm();
        form.AddField("chain", _chain);
        form.AddField("network", _network);
        form.AddField("account", _account);
        form.AddField("contract", _contract);
        form.AddField("first", _first);
        form.AddField("skip", _skip);

        string url = host + "/all721";
        UnityWebRequest webRequest = UnityWebRequest.Post(url, form);
        yield return webRequest.SendWebRequest();
        Response<string> data = JsonUtility.FromJson<Response<string>>(System.Text.Encoding.UTF8.GetString(webRequest.downloadHandler.data));
        OnErcReceived(data.response, clction);
    }
    private void OnErcReceived(string json, Collection collection)
    {
        ResetCollection(collection);
        var nfts = JsonConvert.DeserializeObject<List<NFT>>(json);
        for (int i = 0; i < nfts.Count; i++)
        {
            if (collection == Collection.GOBots)
                StartCoroutine(DownloadMetadata<GOBot>(nfts[i], (bot) => { if (bot != null) dataContainer.gobots.Add(bot); }));
            else
                StartCoroutine(DownloadMetadata<WNFT>(nfts[i], (card) => { if (card != null) dataContainer.wnfts.Add(card); }));
        }
        if (collection == Collection.GOBots)
            OnUserGOBotsReceived?.Invoke(JsonConvert.SerializeObject(dataContainer.gobots));
        else
            OnUserWNFTsReceived?.Invoke(JsonConvert.SerializeObject(dataContainer.wnfts));
    }
    private void ResetCollection(Collection collection)
    {
        if (collection == Collection.GOBots)
            dataContainer.gobots.Clear();
        else
            dataContainer.wnfts.Clear();

    }
    private IEnumerator DownloadMetadata<T>(NFT nft, Action<T> OnSuccess) where T : class
    {
        UnityWebRequest www = UnityWebRequest.Get(nft.uri);
        yield return www.SendWebRequest();

        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log($"Corrupted metadata");
        }
        else
        {
            var metadata = www.downloadHandler.text;
            OnSuccess(JsonConvert.DeserializeObject<T>(metadata));
        }
    }
}
public enum Collection
{
    GOBots,
    WNFTs
}
#region Classes Ignore
[System.Serializable]
public class NFT
{
    public string contract;
    public string tokenId;
    public string uri;
    public int balance;
}
[System.Serializable]
public class WNFT
{
    public string name;
    public string image;
    public Properties property;
}
[System.Serializable]
public class Properties
{
    public string Level { get; set; }
    public string Life;
    public string HP;
    public string XP;
    [JsonProperty("Well-being")]
    public string Wellbeing;
    [JsonProperty("Favorite Health App")]
    public string FavoriteHealthApp;
    [JsonProperty("Name")]
    public string Name;
    [JsonProperty("Favorite Song")]
    public string FavoriteSong;
    [JsonProperty("Occupation")]
    public string Occupation;
    [JsonProperty("Spiritual Affiliation")]
    public string SpiritualAffiliation;
    [JsonProperty("Fitness Preference")]
    public string FitnessPreference;
    [JsonProperty("Nutritional Type")]
    public string NutrionalType;
    [JsonProperty("Social Preference")]
    public string SocialPreference;
}
[System.Serializable]
public class Attribute
{
    public string trait_type { get; set; }
    public string value { get; set; }
}
[System.Serializable]
public class GOBot
{
    public string name { get; set; }
    public string description { get; set; }
    public string image { get; set; }
    public string externalUrl { get; set; }
    public List<Attribute> attributes { get; set; }
}
#endregion