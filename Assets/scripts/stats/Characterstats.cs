using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
public enum StatType
{
    strength,
    agility,
    intelegence,
    vitality,
    damage,
    critChance,
    critPower,
    health,
    armor,
    evasion,
    magicRes,
    fireDamage
}
public class Charactorstats : MonoBehaviour
{
   
    private entityFX fx;
    [Header("major stats")]
    public Stats strength;
    public Stats intelligence;
    public Stats vitality;
    public Stats agility;

    [Header("offensive stats")]
    public Stats damage;
    public Stats critchance;
    
    public Stats critpower;


    [Header("defensive stats")]
    public Stats maxHP;
    public Stats evasion;
    public Stats armor;
    public Stats magicrisistance;

    [Header("magic stats")]
    public Stats firedamage;
    public Stats icedamage;
    public Stats lighteningdamage;
    public bool isignited=false;
    public bool ischilled=false;
    public bool isshocked = false;
    [Header("fireinfo")]
    public float firetime;
    public float offfiretime;
    private float firedur=0.3f;
    private float offfiredur = 5f;
    [Header("shockinfo")]
    public float shocktime;
    public float offshocktime;
    public float shockdur ;
    private float offshockdur = 5f;

  



    [Header("hp")]
    public int currenthealth;
    protected virtual void Start()
    {
        critpower.setdefaultvalue(150);
        currenthealth = getmaxhealthvalue();
        fx = GetComponent<entityFX>();
      
      
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        dofiredamage(this);
        doicedamage(this);
        doshockdamage(this);
        
    }
    public virtual void increasestatby(int _modifier,float _duration,Stats _statstomodify)
    {
        StartCoroutine(StartModCorountine(_modifier,_duration,_statstomodify));
    }
    private IEnumerator StartModCorountine(int _modifier, float _duration, Stats _statstomodify)
    {
        _statstomodify.Addmodifier(_modifier);
        yield return new WaitForSeconds(_duration);
        _statstomodify.Removemodifier(_modifier);
    }
   public virtual void onfire(Charactorstats _targetstats)
    {
        _targetstats.isignited = true;
       


    }
    public virtual void onchill(Charactorstats _targetstats)
    {
        _targetstats.ischilled= true;



    }
    public virtual void onshock(Charactorstats _targetstats)
    {
        _targetstats.isshocked= true;



    }
    public virtual void heal(Charactorstats _targetstats, int _healnum)
    {
        _targetstats.currenthealth+= _healnum;
    }
    public virtual void dofiredamage(Charactorstats _targetstats)
    {
        if (_targetstats.isignited)
        {
            fx.ignitedfor(offfiredur);
            firetime -= Time.deltaTime;
            offfiretime -= Time.deltaTime;
            if (firetime > 0)
            {
               
            }
            else
            {
                int _firedamage = firedamage.getvalue();
                int totaldamage = _firedamage;
                totaldamage *= checkmagicrisistance(_targetstats, totaldamage);
                totaldamage = Mathf.Clamp(totaldamage, 1, int.MaxValue);
                _targetstats.takedamage(totaldamage);
                firetime = firedur;


            }
            if (offfiretime < 0)
            {
                _targetstats.isignited = false;
                offfiretime = offfiredur;
            }

          
        }
       
        
    }
    public virtual void doicedamage(Charactorstats _targetstats)
    {
        if (_targetstats.ischilled)
        {
            int _icedamage = icedamage.getvalue();
            int totaldamage = _icedamage;
            totaldamage *= checkmagicrisistance(_targetstats, totaldamage);
            totaldamage = Mathf.Clamp(totaldamage, 0, int.MaxValue);

            _targetstats.takedamage(totaldamage);
        }

       

    }
    public virtual void doshockdamage(Charactorstats _targetstats)
    {
        


        if (_targetstats.isshocked )
        {
            shocktime -= Time.deltaTime;
            offshocktime -= Time.deltaTime;
            if (shocktime <= 0)
            {
                fx.shockfxfor(offshockdur);

                int _lightdamage = lighteningdamage.getvalue();
                int totaldamage = _lightdamage;
                totaldamage *= checkmagicrisistance(_targetstats, totaldamage);
                totaldamage = Mathf.Clamp(totaldamage, 0, int.MaxValue);
                _targetstats.takedamage(totaldamage);
                shocktime = shockdur;
            }
          
        }
        if (offshocktime < 0)
        {
            shocktime = shockdur;
          
            offshocktime = offshockdur;
            _targetstats.isshocked = false;
        }
      
    }




  

    private int checkmagicrisistance(Charactorstats _targets,int _totaldamage)
    {
        _totaldamage = (1 - magicrisistance.getvalue());
        return _totaldamage;
    }

    public virtual void dodamage(Charactorstats _targetstats)
    {
        if (canavoidattack(_targetstats))
        {
            return;
        }
        
        int totaldamage = damage.getvalue() + strength.getvalue();
        totaldamage -= _targetstats.armor.getvalue();
        if (totaldamage < 1)
        {
            totaldamage = 1;
        }
        if (cancrit(_targetstats))
        {
            totaldamage = calculatecriticaldamage(totaldamage);
        }
         _targetstats.takedamage(totaldamage);
        
    }

    private bool cancrit(Charactorstats _targetstats)
    {
        int totalcrit = critchance.getvalue();
        if (Random.Range(0, 100) < totalcrit)
        {
            Debug.Log("crit");
            return true;
        }
        return false;
    }


    public virtual void takedamage(int _damage)
    {
        currenthealth -= _damage;
        Debug.Log(_damage);
    }
    private bool canavoidattack(Charactorstats _targetstats)
    {
        int totalevasion = _targetstats.agility.getvalue() + _targetstats.evasion.getvalue();
        if (Random.Range(0, 100) < totalevasion)
        {
            Debug.Log("evasion!");
            return true;
        }
        return false;
    }
    private int calculatecriticaldamage(int _damage)
    {
        float criticalpower = (critpower.getvalue() + strength.getvalue())*0.01f;
        float crittotaldamage = criticalpower * _damage;
        return Mathf.RoundToInt(crittotaldamage);
    }
    public int getmaxhealthvalue()
    {
        return maxHP.getvalue() + vitality.getvalue();
    }
    public Stats StatToModify(StatType buffType)
    {
        if (buffType == StatType.strength) return strength;
        else if (buffType == StatType.agility) return agility;
        
        else if (buffType == StatType.vitality) return vitality;
        else if (buffType == StatType.damage) return damage;
        
        else if (buffType == StatType.health) return maxHP;
        else if (buffType == StatType.armor) return armor;
        else if (buffType == StatType.evasion) return evasion;
    

        return null;
    }
}
