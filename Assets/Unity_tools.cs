using UnityEngine;
using System.Collections;
using System;



public enum Self_State_Enmu
{
    idle = 0,
    move = 1,
    attack = 2,
};

public enum Attack_Dir_Enmu
{
    right = 1,
    left = 2,
    high = 3,
    low = 4,
};


public class Attack_Parameter
{
    public Attack_Dir_Enmu attack_dir;



    public Attack_Parameter()
    {
        attack_dir = Attack_Dir_Enmu.high;
    }


    public Attack_Dir_Enmu update_radom_value()
    {
        int min = Convert.ToInt32(Attack_Dir_Enmu.right);
        int max = Convert.ToInt32(Attack_Dir_Enmu.low)+1;
        attack_dir = (Attack_Dir_Enmu)UnityEngine.Random.Range(min, max);
        return attack_dir;
    }



}
