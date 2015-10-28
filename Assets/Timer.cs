using UnityEngine;
using System.Collections;

public class Timer
{

    public delegate void Timer_Delegate();

    public static IEnumerator Timer_IEnumerator(float interval, Timer_Delegate Timer_Delegate_Fuc)
    {
        while (true)
        {
            Timer_Delegate_Fuc();
            yield return new WaitForSeconds(interval);
        }
    }
}
