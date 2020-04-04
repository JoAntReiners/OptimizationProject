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
    /*----------------------------------------Matthew Sankey added variable--------------------------------------*/
    private float MaxGold = 2147483647; //the limit for integers is 2,147,483,647
    #endregion


    void Start()
    {
        //Setting it up so that when Gold is Earned Progress is Saved and the Text is Updated

        OnGoldEarnedCallback += UpdateGoldText; //UpdateGoldText should replace this
        
        //OnGoldEarnedCallback += SaveProgress;
        /*-------------------------------------Commented out this---------------------------------------------*/

        //Loading previous progress if such progress exists

        LoadProgress();
    }

    void Update()
    {
        //Calls the Gold Per Second Method to continue updating the amount of gold the player has

        GoldPerSecondCalc();   
    }

    public void CoinButton()
    {
        //Adds One Gold to the Total Count, then Checks to see if the player has enough to buy any upgrades
        //Afterwards it invokes the Callback which in turn updates the UI

        if (CurrentGold < MaxGold)  /*-----------------------------Matthew Sankey added a cap here------------------------*/
        {
            CurrentGold++;
        }
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    public void BuyDRank()
    {
        //Subtracts the cost of the upgrade, updates the gold per second value to whatever it was previously
        //plus the value of the upgrade, updates the amount of said upgrade owned, then updates text
        //the button states, and invokes the callback

        CurrentGold -= DRankCost;
        GoldPerSecond += DRankGPS;
        DRankOwned++;
        UpdateOwnedText();
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    public void BuyCRank()
    {
        //Subtracts the cost of the upgrade, updates the gold per second value to whatever it was previously
        //plus the value of the upgrade, updates the amount of said upgrade owned, then updates text
        //the button states, and invokes the callback

        CurrentGold -= CRankCost;
        GoldPerSecond += CRankGPS;
        CRankOwned++;
        UpdateOwnedText();
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    public void BuyBRank()
    {
        //Subtracts the cost of the upgrade, updates the gold per second value to whatever it was previously
        //plus the value of the upgrade, updates the amount of said upgrade owned, then updates text
        //the button states, and invokes the callback

        CurrentGold -= BRankCost;
        GoldPerSecond += BRankGPS;
        BRankOwned++;
        UpdateOwnedText();
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    private void SaveProgress()
    {
        //Saves the Current Gold, Gold Per Second, and Upgrades Owned values so they can be accessed upon returning to the game

        PlayerPrefs.SetFloat("CurrentGold", CurrentGold);
        PlayerPrefs.SetFloat("GoldPerSecond", GoldPerSecond);
        PlayerPrefs.SetInt("DRankOwned", DRankOwned);
        PlayerPrefs.SetInt("CRankOwned", CRankOwned);
        PlayerPrefs.SetInt("BRankOwned", BRankOwned);
    }

    private void LoadProgress()
    {
        //Loads previous values

        CurrentGold = PlayerPrefs.GetFloat("CurrentGold");
        GoldPerSecond = PlayerPrefs.GetFloat("GoldPerSecond");
        DRankOwned = PlayerPrefs.GetInt("DRankOwned");
        CRankOwned = PlayerPrefs.GetInt("CRankOwned");
        BRankOwned = PlayerPrefs.GetInt("BRankOwned");

        //Checks to see if there is a TimeQuit string
        if(!PlayerPrefs.GetString("TimeQuit", "None").Equals("None"))
        {
            //Gets the Tick value that the player quit at, then subtracts it from the current tick value
            //Then gets the elapsed seconds and gets the gold that was earned over that time and adds it
            //to the current gold amount

            long TimeQuit = long.Parse(PlayerPrefs.GetString("TimeQuit"));
            long TimeElapsed = System.DateTime.Now.Ticks - TimeQuit;
            TimeSpan ElapsedTime = new TimeSpan(TimeElapsed);

            CurrentGold += (float)(ElapsedTime.TotalSeconds * GoldPerSecond);
        }

        //Updating UI and the Button states due to the fact that Values possibly have changed 
        //due to the loading

        UpdateGoldText();
        CheckButtons();
        UpdateOwnedText();
    }

    private void UpdateGoldText()
    {
        //Updates the Main Gold Text
        GoldText.text = "You have\n" + (int)CurrentGold + "\nGold";
    }

    private void UpdateOwnedText()
    {
        //Updates the Amount of Each Upgrade Owned Text

        DRankOwnedText.text = "Owned: " + DRankOwned;
        CRankOwnedText.text = "Owned: " + CRankOwned;
        BRankOwnedText.text = "Owned: " + BRankOwned;
    }

    private void GoldPerSecondCalc()
    {
        //Adds the Gold Per Second Value to Current Gold once per second

        //then updates button states and invokes the Callback
        if (CurrentGold < MaxGold) /*-----------------------------Matthew Sankey added a cap here------------------------*/
        {
            CurrentGold += (GoldPerSecond * Time.deltaTime);
        }
        CheckButtons();
        OnGoldEarnedCallback.Invoke();
    }

    private void CheckButtons()
    {
        //Checks to see if the player can afford the upgrade, if they can enables the button
        //if they can't disables the button

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

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
        {
            SaveProgress();

            //Gets the current tick when the application is closed and saves it as a string for future loading

            PlayerPrefs.SetString("TimeQuit", System.DateTime.Now.Ticks.ToString());
        }
        else
        {
            LoadProgress();
        }
    }

    private void OnApplicationPause(bool pause)
    {
        SaveProgress();

        //Gets the current tick when the application is closed and saves it as a string for future loading

        PlayerPrefs.SetString("TimeQuit", System.DateTime.Now.Ticks.ToString());
    }

    private void OnApplicationQuit()
    {
        /*------------------------Matthew Sankey moved saved progress--------------------------------*/
        SaveProgress();

        //Gets the current tick when the application is closed and saves it as a string for future loading

        PlayerPrefs.SetString("TimeQuit", System.DateTime.Now.Ticks.ToString());
    }
}
