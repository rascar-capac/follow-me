using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pedestals : BaseMonoBehaviour
{
    public List<PedestalStoneMatch> PedestalStoneMatches = null;

    private PlayerInventory _PlayerInventory;
    private bool _IsMatching;
    private ItemData _StoneInHand;
    private Transform _StoneInHandPedestal;

    protected override void Start()
    {
        base.Start();
        _PlayerInventory = ((GameObject)ObjectsManager.I["Player"]).GetComponent<PlayerInventory>();
        _PlayerInventory.onItemSwaped.AddListener(SwapStones);
    }

    private void SwapStones(Item focusedPedestal)
    {
        Item focusedStone = null;
        if(focusedPedestal.transform.childCount > 0)
        {
            focusedStone = focusedPedestal.transform.GetChild(0).GetComponent<Item>();
        }
        if(_StoneInHand)
        {
            if(focusedStone)
            {
                focusedStone.transform.SetPositionAndRotation(_StoneInHandPedestal.position, _StoneInHandPedestal.rotation);
                focusedStone.transform.SetParent(_StoneInHandPedestal);
            }
            Instantiate(_StoneInHand._itemBasePrefab, focusedPedestal.transform);
            _StoneInHand = null;
            CheckMatching();
        }
        else if(focusedStone)
        {
            _StoneInHand = focusedStone._itemData;
            _StoneInHandPedestal = focusedPedestal.transform;
            Destroy(focusedStone.gameObject);
        }
    }

    private void CheckMatching()
    {
        _IsMatching = true;
        foreach(PedestalStoneMatch match in PedestalStoneMatches)
        {
            GameObject currentStonePrefab = match.Pedestal.transform.GetChild(0).GetComponent<Item>()._itemData._itemBasePrefab;
            GameObject matchingStonePrefab = match.Stone._itemBasePrefab;
            if(currentStonePrefab != matchingStonePrefab)
            {
                _IsMatching = false;
                break;
            }
        }
        if(_IsMatching)
        {
            foreach(PedestalStoneMatch match in PedestalStoneMatches)
            {
                match.Pedestal.transform.GetChild(0).GetComponent<Item>().ActivateItem();
            }
        }
    }
}

[System.Serializable]
public class PedestalStoneMatch
{
    public Item Pedestal;
    public ItemData Stone;
}
