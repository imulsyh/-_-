using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MagicBar : MonoBehaviour
{
    public static int MagicMax;
    public Image PlayerMagicBar;
    // Start is called before the first frame update
    void Start()
    {
        //PlayerMagicBar = GetComponent<Image>();
    }

    // Update is called once per frame
    public void UpdateMagicBar(float currentValue, float maxValue)
    {
        PlayerMagicBar.fillAmount = currentValue / maxValue;
    }

}
