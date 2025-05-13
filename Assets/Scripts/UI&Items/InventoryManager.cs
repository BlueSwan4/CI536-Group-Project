using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    // static exposed instance, so we can use the same trick with the game and battle managers
    public static InventoryManager Instance;

    public GameObject InventoryMenu;
    private bool menuActivated;
    public ItemSlot[] itemSlot;

    // GUI references
    [Header("Item Description Widgets")]
    [SerializeField] private TextMeshProUGUI descriptionHeader;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button useItemButton;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)  && menuActivated)  
        {
            OpenInventory();
        }
        else if (Input.GetKeyDown(KeyCode.I)  && !menuActivated && (GameManager.Instance.State == GameState.Wandering || (GameManager.Instance.State == GameState.Fighting && BattleManager.Instance.State == BattleState.PlayerTurn))) // add check so we can only open the inv in the battle when its our turn or in overworld
        {
            CloseInventory();
        }
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemScriptable itemSO)
    {
       
        for (int i = 0; i < itemSlot.Length; i++)
        {
            Debug.Log("Current Slot Item: " + itemSlot[i].itemName);
            Debug.Log("Name of item to add: " + itemName);
            if ((itemSlot[i].isFull == false && itemSlot[i].itemName == itemName) || itemSlot[i].quantity == 0)
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
        // empty the description text and disable item use button
        descriptionHeader.SetText("");
        descriptionText.SetText("");
        useItemButton.gameObject.SetActive(false);
    }

    public void UseItem()
    {
        // check we should actually be able to use the item
        if (GameManager.Instance.State == GameState.Fighting)
        {
            if (BattleManager.Instance.State != BattleState.PlayerTurn)
            {
                return;
            }
        }

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

        Debug.Log("Chosen item index: " + itemIndex);

        if (itemIndex == -1)
            return;

        // use the item stored at the current index
        itemSlot[itemIndex].slotItemSO.UseItem();

        if(itemSlot[itemIndex].UpdateSlottedItem())
        {
            if (itemIndex < itemSlot.Length - 1)
                itemSlot[itemIndex].MoveSlotData(itemIndex);
        }

        // update item slot full status
        itemSlot[itemIndex].isFull = false;

        // if we're on the player turn, make sure it gets spent
        if (BattleManager.Instance.State == BattleState.PlayerTurn)
        {
            BaseUnit.EndUnitTurn(); // trigger end of turn
            // close inventory screen
            CloseInventory();
        }
    }

    public bool IsInventoryEmpty()
    {
        bool empty = true;

        for (int i = 0; i < itemSlot.Length; i++)
        {
            if (itemSlot[i].quantity != 0)
            {
                empty = false;
                break;
            }
        }

        return empty;
    }

    public void OpenInventory()
    {
        // check what state we're in
        if (GameManager.Instance.State == GameState.Wandering)
        {
            GameManager.Instance.UpdateGameState(GameState.ViewingInventory);
        }

        GetComponent<Canvas>().sortingOrder = 1;
        menuActivated = true;
        InventoryMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void CloseInventory()
    {
        menuActivated = false;
        InventoryMenu.SetActive(false);
        Time.timeScale = 1;
        if (GameManager.Instance.State == GameState.ViewingInventory)
        {
            GameManager.Instance.UpdateGameState(GameState.Wandering);
        }

        // reset z index
        GetComponent<Canvas>().sortingOrder = 0;
    }
}
