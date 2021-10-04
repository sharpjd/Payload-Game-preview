using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineupHandler : MonoBehaviour
{

    public static LineupHandler Instance;
    public List<TeamLineupContainer> TeamLineups = new List<TeamLineupContainer>();

    /*
    public float YGapBetweenItems = -2f;
    public float XGapBetweenLineups = 2f;

    public Vector2 Offset;
    */

    public float LineupUpdateIntervalSeconds = 0.2f;
    

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        Debug.Log("Started Lineup Handler");
    }

    float lastUpdateTime = 0f;
    private void Update()
    {
        if (Time.time - lastUpdateTime > LineupUpdateIntervalSeconds)
        {
            lastUpdateTime = Time.time;

            foreach(TeamLineupContainer teamLineup in TeamLineups)
            {
                teamLineup.UpdateLineupCounts();
            }
        }
    }

    /*
    public void PlaceTeamLineups()
    {

        int currentColumn = 0;
        int currentRow = 0;
        float currentX;
        float currentY;

        int totalColumns = TeamLineups.Count;

        foreach(TeamLineupContainer teamLineupContainer in TeamLineups)
        {
            currentY = 0f;
            currentColumn++;
            currentX = -(totalColumns * XGapBetweenLineups) / 2 + ((currentColumn - 1) * XGapBetweenLineups) + Offset.x;

            foreach(UIShipSpriteContainer uIShipSpriteContainer in teamLineupContainer.LineupUIShipSpriteContainers)
            {
                currentRow++;
                currentY += currentRow * YGapBetweenItems + Offset.y;
                uIShipSpriteContainer.transform.position = new Vector2(Offset.x + currentX, Offset.y + currentY);
                uIShipSpriteContainer.gameObject.SetActive(true);
            }
        }
    }
    */
}
