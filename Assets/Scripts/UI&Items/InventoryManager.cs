using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;

public class InventoryManager : MonoBehaviour
{
    public GameObject InventoryMenu;
    private bool menuActivated;
    [SerializeField]
    public ItemSlot[] itemSlot;
  
  
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)  && menuActivated)  
        {
            GameManager.Instance.UpdateGameState(GameState.Wandering);
            InventoryMenu.SetActive(false);
            menuActivated = false;
            Time.timeScale = 1;
        }
        else if (Input.GetKeyDown(KeyCode.I)  && !menuActivated && GameManager.Instance.State == GameState.Wandering)
        {
            GameManager.Instance.UpdateGameState(GameState.ViewingInventory);
            InventoryMenu.SetActive(true);
            menuActivated = true;
            Time.timeScale = 0;
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemScriptable itemSO)
    {
       
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].isFull == false && itemSlot[i].name == name || itemSlot[i].quantity == 0)
            {
                int leftOverItems = itemSlot[i].AddItem(itemName, quantity, itemSprite, itemDescription, itemSO);
                if (leftOverItems > 0)
                    leftOverItems = AddItem(itemName, leftOverItems, itemSprite, itemDescription, itemSO);


                    return leftOverItems;
            }
        }
        return quantity;
    }

    public void DeselectAllSlots()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectedShader.SetActive(false);
            itemSlot[i].thisItemSelected = false;
        }
    }

    public void UseItem()
    {
        // will be attached to the use button in the inventory screen
        int itemIndex = -1;
        
        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].thisItemSelected)
            {
                itemIndex = i;
                break;
            }
        }

        if (itemIndex == -1)
            return;

        // use the item stored at the current index
        itemSlot[itemIndex].slotItemSO.UseItem();
        if(itemSlot[itemIndex].UpdateSlottedItem())
        {
            if (itemIndex < itemSlot.Length - 1)
                itemSlot[itemIndex].MoveDataToPreviousSlot(itemIndex + 1);
        }
    }
}
