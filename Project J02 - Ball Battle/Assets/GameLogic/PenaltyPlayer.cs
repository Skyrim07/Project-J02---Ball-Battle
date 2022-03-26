using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PenaltyPlayer : MonoBehaviour
{
    public GameObject highlight;
    public Transform ballTF;
    GameObject ball;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (!PenaltyManager.instance.isGameActive)
            return;

        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");
        float angle = Mathf.Atan2(hAxis, vAxis) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0, angle, 0);
        if(hAxis!=0||vAxis!=0)
        transform.Translate(transform.forward * Time.deltaTime * 2f, Space.World);

        if (ball != null)
        {
            ball.transform.position = ballTF.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Ball"))
        {
            ball = other.gameObject;
            highlight.SetActive(true);
        }
    }
}
