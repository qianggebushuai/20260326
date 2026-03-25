using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerLight : MonoBehaviour
{
    private TopDownPlayer player;
    private Light2D lights;
    public bool isOn = true;
    public float ho = 0;
    public float ve = 0;
    public Transform originaltransform;
    void Start()
    {
        player = GetComponentInParent<TopDownPlayer>();
        lights = GetComponent<Light2D>();
    }

    void Update()
    {
        originaltransform = transform.parent.transform;
        if (lights == null) return;

        lights.enabled = isOn;

        if (isOn && player != null)
        {
            float angle = GetAngleFromDirection(player.facingDirection);
            transform.localRotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private float GetAngleFromDirection(FacingDirection direction)
    {
        switch (direction)
        {
            case FacingDirection.Left:transform.position = new Vector2(originaltransform.position.x - ho,originaltransform.position.y);
                return 90f;  
            case FacingDirection.Right:
                transform.position = new Vector2(originaltransform.position.x + ho, originaltransform.position.y);
                return -90f;
            case FacingDirection.Up:
                transform.position = new Vector2(originaltransform.position.x , originaltransform.position.y+ve);
                return 0f;
            case FacingDirection.Down:
                transform.position = new Vector2(originaltransform.position.x , originaltransform.position.y-ve);
                return 180f;
            default: return 180f;
        }
    }
}
