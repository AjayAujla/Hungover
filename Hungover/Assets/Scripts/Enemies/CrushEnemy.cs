using UnityEngine;
using System.Collections;

public class CrushEnemy : BaseEnemy
{
	private Transform target;
	private float exposureTime;
	private float multiplier;
	
	[SerializeField]
	float followRange;
	[SerializeField]
	float followSpeed;
	[SerializeField]
	float suspicionRange;
	
	private SpriteRenderer[] rangeIndicators;
	
	private EmbarrassmentMeter embarrassmentMeter;
	
	void Start()
	{
		this.embarrassmentMeter = GameObject.Find("EmbarassmentMeter").GetComponent<EmbarrassmentMeter>();
		
		if (this.gameObject.name == "TestEnemy")
		{
			this.target = GameObject.Find("AshFlashem(Clone)").transform;
			this.rangeIndicators = this.gameObject.GetComponentsInChildren<SpriteRenderer>();
			
			SpriteRenderer greenZone = this.rangeIndicators[0];
			SpriteRenderer orangeZone = this.rangeIndicators[1];
			SpriteRenderer redZone = this.rangeIndicators[2];
			
			
			float[] sizes = { this.suspicionRange + 2 , this.suspicionRange, this.followRange};
			
			for(int i = 1; i < this.rangeIndicators.Length; ++i)
			{
				float targetSize = sizes[i - 1];
				float currentSize = this.rangeIndicators[i].sprite.bounds.size.x;
				
				Vector3 scale = this.rangeIndicators[i].transform.localScale;
				
				scale.x = targetSize * scale.x / currentSize;
				scale.y = targetSize * scale.y / currentSize;
				
				this.rangeIndicators[i].transform.localScale = scale;
			}
		}
	}
	
	void FixedUpdate()
	{
		if (this.target != null && this.gameObject.name == "TestEnemy")
		{
			
			float distance = (this.target.transform.position - this.transform.position).magnitude;
			//Debug.LogError(distance);
			
			this.embarrassmentMeter.setEmbarrassment(distance);
			this.playerWithinRange();
		}

	}
	
	void playerWithinRange()
	{
		if (this.target != null && this.gameObject.name == "TestEnemy")
		{
			Vector3 directionToPlayer = this.target.transform.position - this.transform.position;
			
			if (directionToPlayer.magnitude <= this.followRange) // Red Zone
			{
				this.followSpeed = 1.5f;
				this.exposureTime += Time.deltaTime;
				this.multiplier = 0.1f;
			}
			else if (directionToPlayer.magnitude <= this.suspicionRange) // Yellow Zone 
			{
				this.followSpeed = 1.0f;
				this.exposureTime += Time.deltaTime;
				this.multiplier = 0.05f;
			}
			else // Green Zone
			{
				this.multiplier = 0.0f;
			}
		}
	}
}