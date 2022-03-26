using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenaltyBall : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Gate"))
        {
            PenaltyManager.instance.OnGoal();
        }
    }
}
