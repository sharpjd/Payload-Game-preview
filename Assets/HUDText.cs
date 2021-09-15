using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HUDText : MonoBehaviour
{

    public static HUDText Instance;

    public GameObject UIBottomDisplay;

    public GameObject UIInstructions;

    [HideInInspector]
    public TextMeshProUGUI InstructionsText;
    [HideInInspector]
    public TextMeshProUGUI BottomDisplayText;

    public LevelHandler LevelHandler;

    public  AllegianceInfo EnemyFaction;

    public float UIScoreUpdateIntervalSeconds = 0.2f;

    [TextArea]
    public string PreparingInstructions;

    private void Start()
    {

        Instance = this;

        Debug.Log("Started on-screen instructions handler");

        InstructionsText = UIInstructions.GetComponent<TextMeshProUGUI>();
        BottomDisplayText = UIBottomDisplay.GetComponent<TextMeshProUGUI>();

    }

    bool preparing;
    bool playing;

    float lastUpdateTime = 0f;
    private void Update()
    {

        if(LevelHandler.LevelState == LevelState.Preparing)
        {
            if (preparing) goto Preparing;
            preparing = true;
            InstructionsText.SetText(PreparingInstructions);

            Preparing:

            InstructionsText.gameObject.SetActive(true);
            BottomDisplayText.SetText("phase 1 \n instantiate units (" + LevelHandler.Instance.FriendlyBank + " to spend)");
            
        }
        
        if(LevelHandler.LevelState == LevelState.Started)
        {

            if (playing) goto UpdateScore;

            InstructionsText.gameObject.SetActive(false);
            playing = true;

            UpdateScore:

            if (Time.time - lastUpdateTime > UIScoreUpdateIntervalSeconds)
            {

                lastUpdateTime = Time.time;

                int countA = Targets.GetAllTargetsOfFactionRemoveInvalids(GameState.Instance.PlayerAllegiance.Faction, ShipType.Missile).Count;
                int countB = Targets.GetAllTargetsOfFactionRemoveInvalids(EnemyFaction.Faction, ShipType.Missile).Count;

                if(countA == countB)
                    BottomDisplayText.SetText(countA + "  :  " + countB);
                if (countA > countB)
                    BottomDisplayText.SetText(countA + "  :  " + countB);
                if (countA < countB)
                    BottomDisplayText.SetText(countA + "  :  " + countB);
            }

        }

        if(LevelHandler.LevelState == LevelState.Victory)
        {
            BottomDisplayText.SetText("success");
        }

        if (LevelHandler.LevelState == LevelState.Defeat)
        {
            BottomDisplayText.SetText("payload nullified");
        }

    }




}
