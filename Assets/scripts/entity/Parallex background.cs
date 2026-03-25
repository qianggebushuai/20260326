using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallexbackground : MonoBehaviour
{
    private GameObject cam;
    [SerializeField] private float parallexeffect;
    private float xPosition;
    private float length;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        length = GetComponent<SpriteRenderer>().bounds.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distancemoved = cam.transform.position.x * (1 - parallexeffect);
        float distanceToMove = cam.transform.position.x *parallexeffect;
        transform.position = new Vector3(xPosition +distanceToMove, transform.position.y);
        if (distancemoved >(xPosition +length) )
        {
            xPosition = xPosition + length;
        }else if(distancemoved < xPosition - length)
        {
            xPosition = xPosition - length;
        }
    }
}
