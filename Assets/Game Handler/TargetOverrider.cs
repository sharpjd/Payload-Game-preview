using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
public class TargetOverrider : MonoBehaviour
{
    public static TargetOverrider Instance;

    public float MinimumSelectionEdgeSize = 0.5f;

    public float clickStartX;
    public float clickStartY;

    public Entity CurrentMouseOverEntity;

    bool dragging = false;
    bool selectionInitiated = false;

    public List<Entity> selectedEntities = new List<Entity>();

    public LineRenderer lineRenderer;



    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        Debug.Log("Started TargetOverrider");

        lineRenderer ??= GetComponent<LineRenderer>();
        lineRenderer ??= gameObject.AddComponent<LineRenderer>();

    }

    /*
     * This is some really foul code — note that the way targeting is handled is actually spread over this class and the Entity class
     * I wrote this when I was in a time crunch but it's here to stay and not worth rewriting because how often is this class going
     * to be needed to modified?
     */

    //used to prevent acquiring animation from playing every Update() causing a bazillion animations to occur
    bool targetedBeforeMouseRelease = false;
    void OnGUI()
    {

        lineRenderer.positionCount = 0;

        /*
         * used to prevent acquiring animation from playing every Update() causing a bazillion animations/overrides to occur; set to true when
         * a target is acquired, and prevents any more targeting actions from executing until right mouse is released, in which
         * then targetedBeforeMouseRelease is set to false (allowing rest of function to proceed)
         */
        if (Input.GetKey(KeyCode.Mouse1) && targetedBeforeMouseRelease)
        {
            return;
        } 
        else
        {
            targetedBeforeMouseRelease = false;
        }

        /* 
         * If selection box not visible to select targets & leftmouse down & shift not down & mouse is not over an entity, clear list of selected;
         * in selection UI, this is de-selecting everything you've selected by clicking once
         */

        if (!selectionInitiated && Input.GetKeyDown(KeyCode.Mouse0) && !Input.GetKey(KeyCode.LeftShift) && CurrentMouseOverEntity == null)
        {
            //loop through selectedEntities and trigger each Entity instance's OnDeselectAndRemoveFromSelected()
            ClearSelectedList();
        }

        bool leftMouseDown = Input.GetKey(KeyCode.Mouse0);
        bool rightMouseDown = Input.GetKey(KeyCode.Mouse1);

        //if not already dragging, set the selection box X and Y to mouse position in world as well as dragging to true
        if (!dragging && leftMouseDown)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            clickStartX = mousePos.x;
            clickStartY = mousePos.y;
            dragging = true; 
        }

        if (dragging && leftMouseDown)
        {
            //redundant — but not removing just in case it breaks something
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            //if selection box (selection start pos - mouse pos) is more than 0.5 units large in world 
            if (Mathf.Abs(clickStartX - mousePos.x) > 0.5f || Mathf.Abs(clickStartY - mousePos.y) > 0.5f)
            {

                selectionInitiated = true;

                int layermask = 1 << 2;
                layermask = ~layermask;

                Collider2D[] contacts = Physics2D.OverlapAreaAll(new Vector2(clickStartX, clickStartY), new Vector2(mousePos.x, mousePos.y), layermask);

                /*
                 * for everything in rectangular selection area, call each Entity's OnSelectAndTryAddToSelected(); if its faction is the player's &
                 * if it is selectable & if it's not currently in selected list, it will be added to the selected list; jump to the aforementioned
                 * function to see in more detail
                 */
                for(int i = 0; i < contacts.Length; i++)
                {
                    Entity entity = contacts[i].gameObject.GetComponent<Entity>();

                    if (entity != null && entity.Selectable && entity.AllegianceInfo.Faction == GameState.Instance.PlayerAllegiance.Faction)
                    {
                        entity.OnSelectAndTryAddToSelected();
                        //playerFactionEntities.Add(entity);
                    }
                }

                //update the appearance of the selection rectangle
                Rect rect;

                if(clickStartX < 0 || clickStartY < 0)
                {
                    rect = new Rect(clickStartX, clickStartY, mousePos.x - clickStartX, mousePos.y - clickStartY);
                } else
                {
                    rect = new Rect(clickStartX, clickStartY, mousePos.x - clickStartX, mousePos.y - clickStartY);
                }

                Vector3[] vector3s = new Vector3[5];

                vector3s[0] = new Vector3(rect.x, rect.y);
                vector3s[1] = new Vector3(rect.x, rect.y + rect.height);
                vector3s[2] = new Vector3(rect.x + rect.width, rect.y + rect.height);
                vector3s[3] = new Vector3(rect.x + rect.width, rect.y);
                vector3s[4] = new Vector3(rect.x, rect.y);

                lineRenderer.positionCount = 5;

                lineRenderer.SetPositions(vector3s);

                /*
                Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x + rect.width, rect.y), Color.green);
                Debug.DrawLine(new Vector3(rect.x, rect.y), new Vector3(rect.x, rect.y + rect.height), Color.red);
                Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x + rect.width, rect.y), Color.green);
                Debug.DrawLine(new Vector3(rect.x + rect.width, rect.y + rect.height), new Vector3(rect.x, rect.y + rect.height), Color.red);
                */
            }
        }

        if(dragging && !leftMouseDown)
        {

            dragging = false;

            if (selectionInitiated)
                selectionInitiated = false;
        }

        if (rightMouseDown && Targets.IsValidTarget(CurrentMouseOverEntity))
        {
            List<AIModule> aiModulesToTryOverride = new List<AIModule>();
            //get all the AIModules of currently selected Entities
            for (int i = 0; i < selectedEntities.Count; i++)
            {
                if(selectedEntities[i] != null)
                {
                    Component[] AIModules = selectedEntities[i].GetComponents(typeof(AIModule));

                    for (int j = 0; j < AIModules.Length; j++)
                    {
                        aiModulesToTryOverride.Add((AIModule)AIModules[j]);
                    }
                }
            }

            //try set overriden target of above AIModules to current mouse over entity (only if it's a target the Entity can attack by comparing its AllegianceInfo)
            if (TryOverrideTargets(aiModulesToTryOverride, CurrentMouseOverEntity))
            {
                //prevent acquiring animation from playing every Update() causing a bazillion animations to occur
                targetedBeforeMouseRelease = true;
                //play target acquired animation
                GameObject gb = Instantiate(GameState.Instance.TargetGameObject, CurrentMouseOverEntity.transform);
            }
        }
    }

    //try set overriden target to current mouse over entity (only if it's a target the Entity can attack by comparing its AllegianceInfo); true if successful, else false
    bool TryOverrideTargets(List<AIModule> aIModules, Entity targetToApe)
    {
        bool isSuccessful = false;
        if(aIModules.Count != 0)
        {
            for(int i = 0; i < aIModules.Count; i++)
            {
                if (aIModules == null)
                    continue;

                if (aIModules[i].PEntity.AllegianceInfo.CanHitIgnoresID(CurrentMouseOverEntity.AllegianceInfo.Faction))
                {
                    if (aIModules[i].PreventTargetOverride == false)
                    {
                        aIModules[i].Target = targetToApe;
                        aIModules[i].TargetingOverriden = true;
                        isSuccessful = true;
                    }
                }
            }
        }
        return isSuccessful;
    }

    void ClearSelectedList()
    {
        for (int i = selectedEntities.Count - 1; i >= 0; i--)
        {
            if (selectedEntities[i] != null)
            {
                selectedEntities[i].OnDeselectAndRemoveFromSelected();
            }
        }
    }

}
