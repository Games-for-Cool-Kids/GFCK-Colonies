using System;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public PlayerHand PlayerHand { get; private set; }
    public BuildHand BuildHand { get; private set; }
    public RemoveBuildingHand RemoveBuildingHand { get; private set; }

    private GameObject _currentHandObject;

    public enum HandOption
    {
        PLAYER,
        BUILD,
        REMOVE_BUILDING,
    }


    void Start()
    {
        PlayerHand = GameObject.Find(GlobalDefines.playerHandName).GetComponent<PlayerHand>();
        BuildHand = GameObject.Find(GlobalDefines.buildHandName).GetComponent<BuildHand>();
        RemoveBuildingHand = GameObject.Find(GlobalDefines.buildHandName).GetComponent<RemoveBuildingHand>();

        Debug.Assert(PlayerHand != null);
        Debug.Assert(BuildHand != null);

        BuildHand.StructurePlaced += ActivatePlayerHand;
        BuildHand.BuildCanceled += ActivatePlayerHand;
        RemoveBuildingHand.BuildingRemoved += ActivatePlayerHand;

        ActivateHand(HandOption.PLAYER);
    }

    public void ActivateHand(HandOption hand)
    {
        PlayerHand.gameObject.SetActive(false);
        BuildHand.gameObject.SetActive(false);

        switch(hand)
        {
            case HandOption.BUILD:
                _currentHandObject = BuildHand.gameObject;
                _currentHandObject.GetComponent<BuildHand>().enabled = true;
                _currentHandObject.GetComponent<RemoveBuildingHand>().enabled = false;
                break;
            case HandOption.REMOVE_BUILDING:
                _currentHandObject = BuildHand.gameObject;
                _currentHandObject.GetComponent<BuildHand>().enabled = false;
                _currentHandObject.GetComponent<RemoveBuildingHand>().enabled = true;
                break;
            case HandOption.PLAYER:
                _currentHandObject = PlayerHand.gameObject;
                break;
            default:
                Debug.LogWarning(String.Format("Hand of type {0} not implemented", hand.ToString()));
                break;
        }

        _currentHandObject.gameObject.SetActive(true);
    }

    private void ActivatePlayerHand(object sender, EventArgs e)
    {
        ActivateHand(HandOption.PLAYER);
        var buildHand = BuildHand.GetComponent<BuildHand>();
    }
    public void ActivateRemoveBuildingHand()
    {
        ActivateHand(HandOption.REMOVE_BUILDING);
    }

    public void Build(GameObject structure)
    {
        ActivateHand(HandOption.BUILD);

        var buildHand = BuildHand.GetComponent<BuildHand>();
        buildHand.SelectStructure(structure);
    }
}
