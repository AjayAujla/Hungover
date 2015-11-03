using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    private int cash;
    private int experience;
    private int beerCans;

    private Text[] playerStatText;
    private Text cashText;
    private Text experienceText;
    private Text beerCansText;

    public void setCash(int cash)
    {
        this.cash = cash;
    }

    public void setExperience(int experience)
    {
        this.experience = experience;
    }

    public void setBeerCans(int beerCans)
    {
        this.beerCans = beerCans;
    }

    /**
     *  Acquiring handle on text components.
     */
    void Start()
    {
        this.playerStatText = this.gameObject.GetComponentsInChildren<Text>();
        this.cashText = this.playerStatText[0];
        this.experienceText = this.playerStatText[1];
        this.beerCansText = this.playerStatText[3];
    }

    /**
     *  Updating text components with new values.
     */
    void Update()
    {
        this.cashText.text = "$" + this.cash.ToString();
        this.experienceText.text = this.experience.ToString();
        this.beerCansText.text = this.beerCans.ToString();
    }
}