using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{

    private float cash;
    private int experience;
    private int beerCans;
    private float timer = 5.0f * 60.0f; //60.0f is conversion to seconds. only modify minutes

    private Text[] playerStatText;
    private Text cashText;
    private Text experienceText;
    private Text beerCansText;
    private Text timerText;

    private EmbarrassmentMeter embarassmentMeter;

    public void setCash(float cash)
    {
        this.cash = cash;
    }

    public void setExperience(int experience)
    {
        this.experience = experience;
    }

    public void incrementExperience(int exp)
    {
        this.experience += exp;
    }

    public void setBeerCans(int beerCans)
    {
        this.beerCans = beerCans;
    }

    public void incrementCash(float cash)
    {
        this.cash += cash;
    }

    public void incrementBeerCans()
    {
        ++this.beerCans;
    }

    public bool timerEnded()
    {
        return this.timer <= 0.0f;
    }

    void Awake()
    {
        this.embarassmentMeter = this.transform.root.GetComponent<EmbarrassmentMeter>();
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
        this.timerText = this.playerStatText[4];

        if (Application.loadedLevelName == "WeddingParty")
        {
            this.timerText.enabled = true;
        }
        else
        {
            this.gameObject.GetComponentsInChildren<Image>()[3].enabled = false;
            this.timerText.enabled = false;
        }
    }

    /**
     *  Updating text components with new values.
     */
    void Update()
    {
        this.cashText.text = "$" + this.cash.ToString("F");
        this.experienceText.text = this.experience.ToString();
        this.beerCansText.text = this.beerCans.ToString();

        if (Application.loadedLevelName == "WeddingParty")
        {
            this.timer -= Time.deltaTime;

            if (this.timer <= 30.0f)
            {
                this.timerText.color = Color.red;
            }

            if (this.timerEnded())
            {
                this.timerText.text = "Time to Get Married!";
                GameObject.Find("AshFlashem(Clone)").GetComponent<Player>().PlayerDieOfEmbarrassment();
            }
            else
            {
                string minutes = Mathf.Floor(this.timer / 60.0f).ToString("00");
                string seconds = (this.timer % 60.0f).ToString("00");
                this.timerText.text = minutes + ":" + seconds;
            }
        }
    }

	void OnGUI()
	{
		if(this.timerEnded()) {
			Application.LoadLevel("WeddingPartyEnd");
		}
	}

    public void checkForUpgrades()
    {
        // maximum 145 exp per level (shirt, pants, shoes, boxers, phone, wallet)
        // minimum 90 exp per level (shirt, pants, shoes)

        // maximum 725 exp per game (5 levels)
        // minimum 450 exp per game (5 levels)

        // 145 290 435 580 725
        // 90 180 270 360 450
        if (this.experience >= 700)
        {
            
        }
        else if (this.experience >= 600)
        {

        }
        else if (this.experience >= 500)
        {

        }
        else if (this.experience >= 400)
        {

        }
        else if (this.experience >= 300)
        {

        }
        else if (this.experience >= 200)
        {

        }
        else if (this.experience >= 100)
        {
            float before = this.embarassmentMeter.getMaximumEmbarrassmentValue();
            this.embarassmentMeter.upgradeEmbarrassmentMeter();
            Utils.Print("upgraded embarrassment from " + before +" to " + this.embarassmentMeter.getMaximumEmbarrassmentValue());
        }
    }

}