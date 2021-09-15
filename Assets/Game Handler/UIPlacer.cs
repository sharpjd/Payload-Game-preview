using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIPlacer : MonoBehaviour
{

    public static UIPlacer Instance;

    public Rect PlacingArea;

    public GameObject PlacingAreaHighlight;

    public GameObject CurrentlySelectedToInstantiate
    {
        get
        {
            return _CurrentlySelectedToInstantiate ? _CurrentlySelectedToInstantiate : null;
        }

        set
        {
            AmountToPlace = 1;
            _CurrentlySelectedToInstantiate = value;
        } 
    }

    [SerializeField]
    private GameObject _CurrentlySelectedToInstantiate = null;

    public int AmountToPlace;

    public GameObject SpawnCountTextObject;
    public Vector2 SpawnCountTextObjectOffset;

    public float SpawnCountTextUpdateIntervalSeconds = 0.1f;

    private void Awake()
    {

    }

    private void Start()
    {
        Instance = this;
        Debug.Log("Started UIPlacer");
        SpawnCountTextObject.SetActive(false);
    }

    private void OnGUI()
    {
        


    }

    float lastUpdateTime = 0f;
    private void Update()
    {

        if (!(LevelHandler.Instance.LevelState == LevelState.Preparing)) return;

        if(AmountToPlace > 1)
        {
            if(Time.time - lastUpdateTime > SpawnCountTextUpdateIntervalSeconds)
            {
                lastUpdateTime = Time.time;
                SpawnCountTextObject.SetActive(true);
                SpawnCountTextObject.GetComponent<TextMeshProUGUI>().SetText(AmountToPlace + "x");
            }   
        }
        else
        {
            if (Time.time - lastUpdateTime > SpawnCountTextUpdateIntervalSeconds)
            {
                lastUpdateTime = Time.time;
                SpawnCountTextObject.SetActive(false);
            }
        }
        
        SpawnCountTextObject.transform.position = new Vector3(Input.mousePosition.x + SpawnCountTextObjectOffset.x, Input.mousePosition.y + SpawnCountTextObjectOffset.y);
    }

}
