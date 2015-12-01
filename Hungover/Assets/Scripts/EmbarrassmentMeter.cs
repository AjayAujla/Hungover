using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EmbarrassmentMeter : MonoBehaviour
{

    private Slider embarrassmentMeterSlider;

    private Sprite greenZone;
    private Sprite yellowZone;
    private Sprite redZone;

    void Start()
    {
        this.embarrassmentMeterSlider = this.GetComponentInChildren<Slider>();

        this.greenZone = Resources.Load<Sprite>("Sprites/green_zone");
        this.yellowZone = Resources.Load<Sprite>("Sprites/yellow_zone");
        this.redZone = Resources.Load<Sprite>("Sprites/red_zone");
    }

    void Update()
    {
        Image embarrassmentMeterFill = this.embarrassmentMeterSlider.GetComponentsInChildren<Image>()[1];
        Image embarrassmentMeterHandleImage = this.embarrassmentMeterSlider.GetComponentsInChildren<Image>()[2];
        if (this.embarrassmentMeterSlider.value <= this.embarrassmentMeterSlider.maxValue / 3.0f)
        {
            embarrassmentMeterFill.color = Color.green;
            embarrassmentMeterHandleImage.sprite = this.greenZone;
        }
        else if (this.embarrassmentMeterSlider.value <= this.embarrassmentMeterSlider.maxValue * 2.0f / 3.0f)
        {
            embarrassmentMeterFill.color = Color.yellow;
            embarrassmentMeterHandleImage.sprite = this.yellowZone;
        }
        else
        {
            embarrassmentMeterFill.color = Color.red;
            embarrassmentMeterHandleImage.sprite = this.redZone;
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

    public float getMaximumEmbarrassmentValue()
    {
        return this.embarrassmentMeterSlider.maxValue;
    }

    public float getMinimumEmbarrassmentValue()
    {
        return this.embarrassmentMeterSlider.minValue;
    }

    public void setEmbarrassmentMeterValue(float value)
    {
        this.embarrassmentMeterSlider.value = value;
    }
}