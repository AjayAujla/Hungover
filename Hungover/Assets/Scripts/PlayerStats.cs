using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour {

    private float cash;
    private int experience;
    private int beerCans;

    private Text[] playerStatText;
    private Text cashText;
    private Text experienceText;
    private Text beerCansText;

    public void setCash(float cash)
    {
        this.cash = cash;
    }

    public void setExperience(int experience)
    {
        this.experience = experience;
    }

	public void incrementExperience(int exp) {
		this.experience += exp;
	}

    public void setBeerCans(int beerCans)
    {
        this.beerCans = beerCans;
    }

	public void incrementCash(float cash) {
		this.cash += cash;
	}

	public void incrementBeerCans() {
		++this.beerCans;
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
		this.cashText.text = "$" + this.cash.ToString("F");
        this.experienceText.text = this.experience.ToString();
        this.beerCansText.text = this.beerCans.ToString();
    }
}