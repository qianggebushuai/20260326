using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class playerhealth : MonoBehaviour
{

    private RectTransform myTransform;
    private Slider slider;
    private Charactorstats mystats;
    private void Start()
    {
        myTransform = GetComponent<RectTransform>();

        slider = GetComponentInChildren<Slider>();
    }
    private void Update()
    {
        updatehealth();
    }
    private void updatehealth()
    {
        slider.maxValue = mystats.getmaxhealthvalue();
        slider.value = mystats.currenthealth;
    }
    
  
}
