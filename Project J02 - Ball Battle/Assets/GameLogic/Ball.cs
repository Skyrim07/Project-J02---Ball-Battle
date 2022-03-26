using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Player player;

    public bool isPassing = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && !isPassing)
        {
            Player otherPlayer = other.GetComponent<Player>();

            if(player==null && otherPlayer.type == PlayerType.Attacker)
            {
                otherPlayer.OnTouchBall();
            }
        }
        if (other.tag.Equals("Gate"))
        {
            FieldManager.instance.OnGoal(other.gameObject);
        }
    }
}
