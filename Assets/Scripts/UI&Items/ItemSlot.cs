using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using System.Runtime.CompilerServices;



public class ItemSlot : MonoBehaviour, IPointerClickHandler
{
    public string itemName;
    public int quantity;
    public Sprite itemSprite;
    public bool isFull;
    public string itemDescription;
    public Sprite emptySprite;


    [SerializeField]
    private TMP_Text quantityText;

    [SerializeField]
    private Image itemImage;

    [SerializeField]
    private int maxNumberOfItems;

    [SerializeField] Button useItemButton;


    public Image itemDescriptionImage;
    public TMP_Text ItemDescriptionNameText;
    public TMP_Text ItemDescriptionText;


    public GameObject selectedShader;
    public bool thisItemSelected;

    public ItemScriptable slotItemSO;

    public InventoryManager inventoryManager;

    private void Start()
    {
        inventoryManager = GameObject.Find("UI").GetComponent<InventoryManager>();
    }

    public int AddItem(string itemName, int quantity, Sprite itemSprite, string itemDescription, ItemScriptable itemDataSO)
    {
        if (isFull)
        {
            return quantity;
        }


        this.itemName = itemName;



        this.itemSprite = itemSprite;

        this.itemDescription = itemDescription;

        slotItemSO = itemDataSO; // use the scriptable object so we can actually call the useitem method

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;

        this.quantity += quantity;
        if (this.quantity >= slotItemSO.stackLimit)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;


            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;

            isFull = true;
            return extraItems;

        }

        quantityText.text = this.quantity.ToString(); 
        quantityText.enabled = true;

        return 0;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            OnLeftClick();
        }
    }

    public void OnLeftClick()
    {
        inventoryManager.DeselectAllSlots();
        selectedShader.SetActive(true);
        thisItemSelected = true;
        ItemDescriptionNameText.text = itemName;
        ItemDescriptionText.text = itemDescription;
        itemDescriptionImage.sprite = itemSprite;
        if (itemDescriptionImage.sprite == null)
        { 
            itemDescriptionImage.sprite = emptySprite;
        }

        useItemButton.gameObject.SetActive(true);
    }

    public bool UpdateSlottedItem(bool reduction = true)
    {
        // returns true if slot is now empty
        Debug.Log("Updating item slot");
        
        if (quantity == 1 && reduction)
        {
            // no items left, clear these fields
            itemName = "";
            quantity = 0;
            slotItemSO = null;
            itemDescriptionImage.sprite = emptySprite;
            quantityText.enabled = false;
            itemDescription = "";

            return true;
        }
        else if (reduction)
        {
            quantity--;
            quantityText.text = quantity.ToString();

            return false;
        }
        else
        {
            // just updating the slot (i.e quantity visibility status) and not reducing item count
            if (quantity != 0)
            {
                quantityText.enabled = true;
                quantityText.text = quantity.ToString();
            }
            else
            {
                quantityText.enabled = false;
            }

            return false;
        }
    }

    public void UpdateItemImage()
    {
        itemImage.sprite = itemSprite;
    }

    public bool MoveDataToPreviousSlot(int slotIndex)
    {
        // make sure to call this on an empty slot object
        Debug.Log("Moving item");
        // use this if a slot before this becomes empty to remove redundant gaps
        if (slotIndex == inventoryManager.itemSlot.Length - 1)
            return true;

        if (inventoryManager.itemSlot[slotIndex].quantity != 0)
        {
            return MoveDataToPreviousSlot(slotIndex + 1);
        }
        else
        {
            // stuff in this slot, move to previous
            if (slotIndex == 0)
                return MoveDataToPreviousSlot(slotIndex + 1);

            inventoryManager.itemSlot[slotIndex - 1].itemName = inventoryManager.itemSlot[slotIndex].itemName;
            inventoryManager.itemSlot[slotIndex - 1].quantity = inventoryManager.itemSlot[slotIndex].quantity;
            inventoryManager.itemSlot[slotIndex - 1].itemSprite = inventoryManager.itemSlot[slotIndex].itemSprite;
            inventoryManager.itemSlot[slotIndex - 1].itemDescription = inventoryManager.itemSlot[slotIndex].itemDescription;

            // update current slot status
            inventoryManager.itemSlot[slotIndex].UpdateSlottedItem(false);

            // update sprites
            inventoryManager.itemSlot[slotIndex].UpdateItemImage();
            inventoryManager.itemSlot[slotIndex - 1].UpdateItemImage();

            // is slot after this empty? or current one the last slot? if so skip this
            if (inventoryManager.itemSlot.Length == slotIndex + 1)
            {
                return true;
            }
            else if (inventoryManager.itemSlot[slotIndex + 1].quantity == 0)
            {
                return true;
            }
            else
            {
                return MoveDataToPreviousSlot(slotIndex + 1);
            }
        }
    }

    public void MoveSlotData(int startSlot)
    {
        // use this alt version maybe? call from emptied slot

        for (int slotIndex = startSlot; slotIndex < inventoryManager.itemSlot.Length; slotIndex++)
        {
            // grab contents of next slot, move it to here
            if (slotIndex == inventoryManager.itemSlot.Length - 1)
            {
                // last slot
                // check if previous slot is empty, if it is clear current slot
                if (inventoryManager.itemSlot[slotIndex - 1].quantity == 0)
                {
                    Debug.Log("At last inventory slot");
                    inventoryManager.itemSlot[slotIndex].itemName = "";
                    inventoryManager.itemSlot[slotIndex].itemDescription = "";
                    inventoryManager.itemSlot[slotIndex].itemSprite = null;
                    inventoryManager.itemSlot[slotIndex].quantity = 0;
                    inventoryManager.itemSlot[slotIndex].slotItemSO = null;

                    inventoryManager.itemSlot[slotIndex].UpdateSlottedItem(false);
                    inventoryManager.itemSlot[slotIndex].UpdateItemImage();
                }
            }
            else
            {
                Debug.Log("Updating slot at index: " + slotIndex);
                inventoryManager.itemSlot[slotIndex].itemName = inventoryManager.itemSlot[slotIndex + 1].itemName;
                inventoryManager.itemSlot[slotIndex].itemDescription = inventoryManager.itemSlot[slotIndex + 1].itemDescription;
                inventoryManager.itemSlot[slotIndex].itemSprite = inventoryManager.itemSlot[slotIndex + 1].itemSprite;
                inventoryManager.itemSlot[slotIndex].quantity = inventoryManager.itemSlot[slotIndex + 1].quantity;
                inventoryManager.itemSlot[slotIndex].slotItemSO = inventoryManager.itemSlot[slotIndex + 1].slotItemSO;

                // update the image
                inventoryManager.itemSlot[slotIndex].UpdateSlottedItem(false);
                inventoryManager.itemSlot[slotIndex].UpdateItemImage();

                // if next slot is empty, break after updating current slot
                if (inventoryManager.itemSlot[slotIndex + 1].quantity == 0)
                {
                    Debug.Log("Next slot is empty");
                    break;
                }
            }
        }

        // deselect all slots
        inventoryManager.DeselectAllSlots();
    }
}