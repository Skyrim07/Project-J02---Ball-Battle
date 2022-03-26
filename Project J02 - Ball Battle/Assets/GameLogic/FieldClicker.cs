using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldClicker : MonoBehaviour
{
    public int fieldID = 0;
    private void OnMouseUpAsButton()
    {
        if (fieldID == 1 && FlowManager.mode == GameMode.PVE)
            return;
        FieldManager.instance.OnClickField(fieldID);
    }
}
