using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using SKCell;
public class BlockProgressBar : MonoBehaviour
{
    public int value;
    const string ANIM_IN_NAME = "HLIN";
    const string ANIM_OUT_NAME = "HLOUT";
    const string ANIM_BOOL_NAME = "Appear";
    public SKSlider slider;

    public Transform highlightContainer;
    private Animator[] highlightAnims;
    void Start()
    {
        highlightAnims = new Animator[highlightContainer.childCount];
        for (int i = 0; i < highlightContainer.childCount; i++)
        {
            highlightAnims[i] = highlightContainer.GetChild(i).GetComponent<Animator>();   
        }
    }

    private void Update()
    {
        value = Mathf.FloorToInt(slider.value * highlightAnims.Length);
        for (int i = 0; i < highlightAnims.Length; i++)
        {
            highlightAnims[i].SetBool(ANIM_BOOL_NAME, value >= i+1);
        }
    }

}
