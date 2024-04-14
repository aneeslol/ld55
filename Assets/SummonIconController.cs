using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SummonIconController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] Image Image;
    [SerializeField] GameObject Tooltip;

    public void OnPointerEnter(PointerEventData eventData)
    {
        Tooltip.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Tooltip.gameObject.SetActive(false);
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public void SetActive(bool active)
    {
        if (active)
            Image.color = Color.white;
        else
            Image.color = new Color(.8f, .8f, 1, .5f);
    }
}
