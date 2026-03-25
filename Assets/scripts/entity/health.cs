using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class health : MonoBehaviour
{
    private entity entity;
    private RectTransform myTransform;
    private Slider slider;
    private Charactorstats mystats;
    private void Start()
    {
        myTransform = GetComponent<RectTransform>();
        entity = GetComponentInParent<entity>();
        slider = GetComponentInChildren<Slider>();
        entity.onflipped += flippedUI;
        mystats = GetComponentInParent<Charactorstats>();
    }
    private void Update()
    {
        updatehealth();
    }
    private void updatehealth()
    {
        slider.maxValue =mystats.getmaxhealthvalue();
        slider.value = mystats.currenthealth;
    }
    private void flippedUI()
    {
      
        myTransform.Rotate(0, 180, 0);
    }
    private void OnDisable()
    {
        entity.onflipped -= flippedUI;
    }
}
