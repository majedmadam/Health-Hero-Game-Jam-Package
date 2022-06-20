using System;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_WEBGL
public class WebLogin : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void Web3Connect();

    [DllImport("__Internal")]
    private static extern string ConnectAccount();

    [DllImport("__Internal")]
    private static extern void SetConnectAccount(string value);

    private int expirationTime;
    private string account;

    public bool connectOnStart;
    public UnityEvent<string> OnUserConnected;

    private void Start()
    {
        if (connectOnStart)
            Login();
    }
    public void Login()
    {
        Web3Connect();
        StartCoroutine(OnConnected());
    }

    private IEnumerator OnConnected()
    {
        account = ConnectAccount();
        while (account == "")
        {
            yield return new WaitForSeconds(1f);
            account = ConnectAccount();
        };
        // save account for next scene
        PlayerPrefs.SetString("Account", account);
        OnUserConnected?.Invoke(account);
        SetConnectAccount("");
        Debug.Log($"Account connected: {account}");
    }

    public void OnSkip()
    {
        // burner account for skipped sign in screen
        PlayerPrefs.SetString("Account", "");
    }
}
#endif
