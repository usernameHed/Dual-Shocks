﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Calculate next ready update
/// <summary>
[System.Serializable]
public class FrequencyTimer
{
	[SerializeField]
	private float updateFrequency;
    [SerializeField]
    private bool NotTheFirstTime = false;

    private float nextUpdate;

	public FrequencyTimer(float updateFrequency)
	{
		this.updateFrequency = updateFrequency;
	}

	public bool Ready()
	{
		if (Time.fixedTime >= nextUpdate)
		{
            if (NotTheFirstTime)
            {
                NotTheFirstTime = false;
                nextUpdate = Time.fixedTime + updateFrequency;
                return (false);
            }
			nextUpdate = Time.fixedTime + updateFrequency;
			return (true);
		}
		return (false);
	}
}
