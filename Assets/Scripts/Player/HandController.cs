using System;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public PlayerHand PlayerHand { get; private set; }
    public BuildHand BuildHand { get; private set; }

    private GameObject _currentHandObject;

    private enum ActiveHand
    {
        PLAYER_HAND,
        BUILD_HAND,
    }


    void Start()
    {
        PlayerHand = GameObject.Find(GlobalDefines.playerHandName).GetComponent<PlayerHand>();
        BuildHand = GameObject.Find(GlobalDefines.buildHandName).GetComponent<BuildHand>();

        Debug.Assert(PlayerHand != null);
        Debug.Assert(BuildHand != null);

        ActivateHand(ActiveHand.PLAYER_HAND);
    }

    private void ActivateHand(ActiveHand hand)
    {
        PlayerHand.gameObject.SetActive(false);
        BuildHand.gameObject.SetActive(false);

        switch(hand)
        {
            case ActiveHand.PLAYER_HAND:
                _currentHandObject = PlayerHand.gameObject;
                break;
            case ActiveHand.BUILD_HAND:
                _currentHandObject = BuildHand.gameObject;
                break;
        }

        _currentHandObject.gameObject.SetActive(true);
    }

    private void ActivatePlayerHand(object sender, EventArgs e)
    {
        ActivateHand(ActiveHand.PLAYER_HAND);
        var buildHand = BuildHand.GetComponent<BuildHand>();
        buildHand.StructurePlaced -= ActivatePlayerHand;
    }

    public void Build(GameObject structure)
    {
        ActivateHand(ActiveHand.BUILD_HAND);

        var buildHand = BuildHand.GetComponent<BuildHand>();
        buildHand.SelectStructure(structure);
        buildHand.StructurePlaced += ActivatePlayerHand;
    }
}
