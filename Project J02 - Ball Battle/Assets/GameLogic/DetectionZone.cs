using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Player player = other.GetComponent<Player>();
            Player self = transform.parent.GetComponent<Player>();
            if (player.side != self.side)
            {
                self.OnTouchDetectionZone(player);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Player player = other.GetComponent<Player>();
            Player self = transform.parent.GetComponent<Player>();
            if (player.side != self.side)
            {
                self.OnExitDetectionZone(player);
            }
        }
    }
}
