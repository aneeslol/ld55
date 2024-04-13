using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ManaBarController : MonoBehaviour
{
    [SerializeField] TMP_Text Text;
    [SerializeField] Image Fill;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void UpdateManaBar(string text, float fill)
    {
        Text.text = text;
        Fill.fillAmount = fill;
    }
}
