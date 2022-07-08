using System;
using UnityEngine;

public class HandController : MonoBehaviour
{
    private GameObject _playerHand;
    private GameObject _buildHand;

    private GameObject _currentHand;
    private GameObject _selectedBuilding;

    private enum ActiveHand
    {
        PLAYER_HAND,
        BUILD_HAND,
    }


    void Start()
    {
        _playerHand = GameObject.Find(GlobalDefines.playerHandName);
        _buildHand = GameObject.Find(GlobalDefines.buildHandName);

        Debug.Assert(_playerHand != null);
        Debug.Assert(_buildHand != null);

        ActivateHand(ActiveHand.PLAYER_HAND);
    }

    private void Update()
    {
        
    }

    private void ActivateHand(ActiveHand hand)
    {
        _playerHand.SetActive(false);
        _buildHand.SetActive(false);

        switch(hand)
        {
            case ActiveHand.PLAYER_HAND:
                _currentHand = _playerHand;
                break;
            case ActiveHand.BUILD_HAND:
                _currentHand = _buildHand;
                break;
        }

        _currentHand.SetActive(true);
    }

    private void ActivatePlayerHand(object sender, EventArgs e)
    {
        ActivateHand(ActiveHand.PLAYER_HAND);
        var buildHand = _buildHand.GetComponent<BuildHand>();
        buildHand.StructurePlaced -= ActivatePlayerHand;
    }

    public void Build(GameObject structure)
    {
        ActivateHand(ActiveHand.BUILD_HAND);

        var buildHand = _buildHand.GetComponent<BuildHand>();
        buildHand.SelectStructure(structure);
        buildHand.StructurePlaced += ActivatePlayerHand;
    }
}
