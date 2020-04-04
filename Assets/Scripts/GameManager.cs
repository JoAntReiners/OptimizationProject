using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    #region Public
    public delegate void OnGoldEarned();
    public OnGoldEarned OnGoldEarnedCallback;
    public TextMeshProUGUI GoldText;
    public TextMeshProUGUI DRankOwnedText;
    public TextMeshProUGUI CRankOwnedText;
    public TextMeshProUGUI BRankOwnedText;
    public Button DRankButton;
    public Button CRankButton;
    public Button BRankButton;
    #endregion

    #region Private
    private float CurrentGold = 0;
    private float GoldPerSecond = 0;
    private int DRankOwned = 0;
    private int CRankOwned = 0;
    private int BRankOwned = 0;
    private float DRankCost = 20;
    private float CRankCost = 5000;
    private float BRankCost = 10000;
    private float DRankGPS = 1;
    private float CRankGPS = 100;
    private float BRankGPS = 500;
    #endregion

    void Start()
    {
        OnGoldEarnedCallback += UpdateGoldText;
        OnGoldEarnedCallback += SaveProgress;
        LoadProgress();
    }

    // Update is called once per frame
    void Update()
    {
        GoldPerSecondCalc();   
    }

    public void CoinButton()
    {
        CurrentGold++;
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    public void BuyDRank()
    {
        CurrentGold -= DRankCost;
        GoldPerSecond += DRankGPS;
        DRankOwned++;
        UpdateOwnedText();
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    public void BuyCRank()
    {
        CurrentGold -= CRankCost;
        GoldPerSecond += CRankGPS;
        CRankOwned++;
        UpdateOwnedText();
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    public void BuyBRank()
    {
        CurrentGold -= BRankCost;
        GoldPerSecond += BRankGPS;
        BRankOwned++;
        UpdateOwnedText();
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    private void SaveProgress()
    {
        PlayerPrefs.SetFloat("CurrentGold", CurrentGold);
        PlayerPrefs.SetFloat("GoldPerSecond", GoldPerSecond);
        PlayerPrefs.SetInt("DRankOwned", DRankOwned);
        PlayerPrefs.SetInt("CRankOwned", CRankOwned);
        PlayerPrefs.SetInt("BRankOwned", BRankOwned);
    }

    private void LoadProgress()
    {
        CurrentGold = PlayerPrefs.GetFloat("CurrentGold");
        GoldPerSecond = PlayerPrefs.GetFloat("GoldPerSecond");
        DRankOwned = PlayerPrefs.GetInt("DRankOwned");
        CRankOwned = PlayerPrefs.GetInt("CRankOwned");
        BRankOwned = PlayerPrefs.GetInt("BRankOwned");

        if(!PlayerPrefs.GetString("TimeQuit", "None").Equals("None"))
        {
            long TimeQuit = long.Parse(PlayerPrefs.GetString("TimeQuit"));
            long TimeElapsed = System.DateTime.Now.Ticks - TimeQuit;
            TimeSpan ElapsedTime = new TimeSpan(TimeElapsed);

            CurrentGold += (float)(ElapsedTime.TotalSeconds * GoldPerSecond);
        }

        UpdateGoldText();
        CheckButtons();
        UpdateOwnedText();
    }

    private void UpdateGoldText()
    {
        GoldText.text = "You have\n" + (int)CurrentGold + "\nGold";
    }

    private void UpdateOwnedText()
    {
        DRankOwnedText.text = "Owned: " + DRankOwned;
        CRankOwnedText.text = "Owned: " + CRankOwned;
        BRankOwnedText.text = "Owned: " + BRankOwned;
    }

    private void GoldPerSecondCalc()
    {
        CurrentGold += (GoldPerSecond * Time.deltaTime);
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    private void CheckButtons()
    {
        if(CurrentGold >= DRankCost)
        {
            DRankButton.interactable = true;
        }
        else
        {
            DRankButton.interactable = false;
        }

        if(CurrentGold >= CRankCost)
        {
            CRankButton.interactable = true;
        }
        else
        {
            CRankButton.interactable = false;
        }

        if(CurrentGold >= BRankCost)
        {
            BRankButton.interactable = true;
        }
        else
        {
            BRankButton.interactable = false;
        }
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetString("TimeQuit", System.DateTime.Now.Ticks.ToString());
    }
}
