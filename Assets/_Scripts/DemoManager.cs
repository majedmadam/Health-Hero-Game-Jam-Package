using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DemoManager : MonoBehaviour
{
    public Button[] nftGetButtons;
    public TMP_Text userAddressT;
    public TMP_Text statusT;
    private void Start()
    {
        foreach (var item in nftGetButtons)
        {
            item.interactable = false;
        }
    }
    public void OnWalletConnected(string userAddress)
    {
        foreach (var item in nftGetButtons)
        {
            item.interactable = true;
        }
        statusT.SetText("Wallet Connected, try the buttons below to get user NFTs");
    }
    public void OnWNFTsReceived(string json)
    {
        var cards = JsonConvert.DeserializeObject<List<WNFT>>(json);
        statusT.SetText($"User W-NFTs received, check the User NFTs data object in data folder. User has ({cards.Count}) W-NFts");
    }
    public void OnGoBotsReceived(string json)
    {
        var bots = JsonConvert.DeserializeObject<List<GOBot>>(json);
        statusT.SetText($"User GO!Bots received, check the User NFTs data object in data folder. User has ({bots.Count}) GO!Bots");
    }
}
