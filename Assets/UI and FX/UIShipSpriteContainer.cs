using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UIShipSpriteContainer : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler//, IPointerClickHandler
{

    public AllegianceInfo AllegianceInfo;
    public Image Image;
    public ShipType ShipType;
    public GameObject ThingToInstantiate;
    public int cost;

    public GameObject TooltipObject;
    public GameObject CountObject;

    [HideInInspector]
    public TextMeshProUGUI UICountText;
    [HideInInspector]
    public TextMeshProUGUI UIToolTipText;

    public float UICountUpdateIntervalSeconds = 0.2f;

    /*
    [TextArea]
    public string Tooltip;
    */

    public float OpacityIfZeroCount = 0.5f;

    public int Count = 0;

    private void Start()
    {
        UICountText = CountObject.GetComponent<TextMeshProUGUI>();
        UIToolTipText = TooltipObject.GetComponent<TextMeshProUGUI>();
    }

    bool mouseover = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        mouseover = true;

        if (LevelHandler.Instance.LevelState == LevelState.Started) return;
        TooltipObject.SetActive(true);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        mouseover = false;
        TooltipObject.SetActive(false);

    }

    float LastTimeUpdatedCount = 0f;

    private void Update()
    {
        if (Count == 0)
        {
            if (AllegianceInfo == null) return;

            Color transparentColor = AllegianceInfo.FactionColor;
            transparentColor.a = OpacityIfZeroCount;

            Image.color = transparentColor;
        }
        else Image.color = AllegianceInfo.FactionColor;


        if(Time.time - LastTimeUpdatedCount > UICountUpdateIntervalSeconds)
        {
            LastTimeUpdatedCount = Time.time;

            UICountText.SetText(Count.ToString());
            UICountText.ForceMeshUpdate(true);
        }
        
        if(mouseover && Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(LevelHandler.Instance.LevelState == LevelState.Preparing)
            {
                if (UIPlacer.Instance.CurrentlySelectedToInstantiate?.GetComponent<Entity>().ShipType == ThingToInstantiate.GetComponent<Entity>().ShipType)
                {
                    UIPlacer.Instance.AmountToPlace++;
                }
                else UIPlacer.Instance.CurrentlySelectedToInstantiate = ThingToInstantiate;
            }
        }
    }

    /*
    public void OnPointerClick(PointerEventData eventData)
    {
        
    }
    */
}




