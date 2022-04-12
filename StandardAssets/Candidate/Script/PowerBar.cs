using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerBar : MonoBehaviour
{
    public Text attackNumText;
    public Text defNumText;
    private void Start()
    {
        MapHandler.instance.changeAxiesNumCallback += UpdateAxiesNum; 
    }

    void UpdateAxiesNum(int attNum, int defNum)
    {
        attackNumText.text = attNum+"";
        defNumText.text = defNum + "";
    }
}
