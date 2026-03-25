using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class entityFX : MonoBehaviour
{

    private SpriteRenderer sr;
    [Header("Flash FX")]
    [SerializeField] private float flashduration;
    [SerializeField] private Material hitmat;
    [SerializeField] private Material originalmat;
    [Header("aliment FX")]
    [SerializeField] private Color chilledcolor;
    [SerializeField] private Color[] ignitedcolor;
    [SerializeField] private Color shockcolor;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originalmat = sr.material;
    }
    private IEnumerator FlashFX()
    {
        sr.material = hitmat;
        Color currentcolor = sr.color;
        sr.color = Color.white;
        yield return new WaitForSeconds(flashduration);
        sr.color = currentcolor;

        sr.material = originalmat;
    }
    private void colorredblink()
    {
        if (sr.color != Color.white)
        {
            sr.color = Color.white;
        }
        else
        {
            sr.color = Color.red;
        }
    }
    private void cancelcolorchange()
    {
        CancelInvoke();
        sr.color = Color.white;
    }
    private void ignitedcolorfx()
    {
        if (sr.color != ignitedcolor[1])
        {
            sr.color = ignitedcolor[1];
        }
        else
        {
            sr.color = ignitedcolor[0];
        }
    }
    private void chillcolorfx()
    {
       
            sr.color = chilledcolor;
       
    }
    private void shockcolorfx()
    {
        sr.color = shockcolor;
    }



    public void ignitedfor(float _seconds)
    {
        InvokeRepeating("ignitedcolorfx", 0, 0.3f);
        Invoke("cancelcolorchange", _seconds);
    }
    public void chillfxfor(float _seconds)
    {
        chillcolorfx();
        Invoke("cancelcolorchange", _seconds);
    }
    public void shockfxfor(float _seconds)
    {
        InvokeRepeating("shockcolorfx", 0, 0.3f);
        Invoke("cancelcolorchange", _seconds);
    }
}
   
