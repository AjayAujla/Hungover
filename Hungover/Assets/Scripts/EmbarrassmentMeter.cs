using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EmbarrassmentMeter : MonoBehaviour {

    private Slider embarrassmentMeterSlider;

    private float maximumCooldownTime = 10.0f;
    private float cooldownTimer;
    private int embarrassmentCooldownDecrement = 1;

    private Sprite greenZone;
    private Sprite yellowZone;
    private Sprite redZone;

    void Start () {
        this.cooldownTimer = this.maximumCooldownTime;

        this.embarrassmentMeterSlider = this.GetComponentInChildren<Slider>();
    }

    void Update () {
		Image embarrassmentMeterImage = this.embarrassmentMeterSlider.GetComponentsInChildren<Image>()[1];
		Image embarrassmentMeterHandleImage = this.embarrassmentMeterSlider.GetComponentsInChildren<Image>()[2];
		if(embarrassmentMeterSlider.value <= 1 && embarrassmentMeterSlider.value >= 0.8) {
			embarrassmentMeterImage.color = Color.green;
		} else if(embarrassmentMeterSlider.value < 0.8 && embarrassmentMeterSlider.value >= 0.4) {
			embarrassmentMeterImage.color = Color.yellow;
		} else {
			embarrassmentMeterImage.color = Color.red;
		}
	}

    private bool embarrassmentMeterFilled()
    {
        return this.embarrassmentMeterSlider.value >= this.embarrassmentMeterSlider.maxValue;
    }

    private bool embarrassmentMeterEmpty()
    {
        return this.embarrassmentMeterSlider.value <= this.embarrassmentMeterSlider.minValue;
    }

    public void increaseEmbarrassment(float value)
    {
        if (this.embarrassmentMeterSlider.value + value >= this.embarrassmentMeterSlider.maxValue)
        {
            this.embarrassmentMeterSlider.value = this.embarrassmentMeterSlider.maxValue;
        }
        else
        {
            this.embarrassmentMeterSlider.value += value;
        }
    }

    private void decreaseEmbarrassment(float value)
    {
        if (this.embarrassmentMeterSlider.value - value <= this.embarrassmentMeterSlider.minValue)
        {
            this.embarrassmentMeterSlider.value = this.embarrassmentMeterSlider.minValue;
        }
        else
        {
            this.embarrassmentMeterSlider.value -= value;
        }
    }

    private void embarrassmentCooldown()
    {
        this.cooldownTimer -= Time.deltaTime;
        if(this.cooldownTimer <= 0.0f)
        {
            this.decreaseEmbarrassment(this.embarrassmentCooldownDecrement);
            this.cooldownTimer = this.maximumCooldownTime;
        }
    }
}