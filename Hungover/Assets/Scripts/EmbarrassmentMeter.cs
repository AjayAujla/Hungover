using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EmbarrassmentMeter : MonoBehaviour {

    private Slider embarrassmentMeterSlider;

    private float maximumCooldownTime = 10.0f;
    private float cooldownTimer;
    private float embarrassmentCooldownDecrement = 1.0f;

    private void prepareState()
    {
        this.cooldownTimer = this.maximumCooldownTime;
    }

    void Start () {
        this.prepareState();

        this.embarrassmentMeterSlider = this.GetComponentInChildren<Slider>();
    }
	
	void Update () {

	}

    private bool embarrassmentMeterFilled()
    {
        return this.embarrassmentMeterSlider.value >= this.embarrassmentMeterSlider.maxValue;
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
        }
    }

}
