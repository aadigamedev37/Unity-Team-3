using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;
using UnityEngine.UI;
using TMPro;    

public class UIShadowManager : MonoBehaviour
{
    public Slider manabar;
    public ShadowManager shadowManager;
    public float manaDecreasetime = 10f; // Rate at which mana decreases when shadow is controlled
    public TextMeshProUGUI ShadowSurface; 
    public TextMeshProUGUI Hazard; 


    void Start()
    {
       manabar.maxValue = 100f;
        manabar.value = manabar.maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        if (shadowManager.shadowMode)
        {
            manabar.value -= (100f / manaDecreasetime) * Time.deltaTime;
            if (manabar.value <= 0f)
            {
                shadowManager.shadowMode = false;
                shadowManager.ExitShadow();
            }
                    
        }
        else
        {
            manabar.value += (100f * 5 / manaDecreasetime) * Time.deltaTime;
        }
    }
}
