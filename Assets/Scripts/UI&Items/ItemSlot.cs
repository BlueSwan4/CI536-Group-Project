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

        isFull = true;

        quantityText.text = quantity.ToString();
        quantityText.enabled = true;
        itemImage.sprite = itemSprite;

        this.quantity += quantity;
        if (this.quantity >= maxNumberOfItems)
        {
            quantityText.text = maxNumberOfItems.ToString();
            quantityText.enabled = true;


            int extraItems = this.quantity - maxNumberOfItems;
            this.quantity = maxNumberOfItems;
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
    }

    public bool UpdateSlottedItem()
    {
        // returns true if slot is now empty
        if (quantity == 1)
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
        else
        {
            quantity--;
            quantityText.text = quantity.ToString();

            return false;
        }
    }

    public bool MoveDataToPreviousSlot(int slotIndex)
    {
        // make sure to call this on an empty slot object

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

            // empty the current slot
            inventoryManager.itemSlot[slotIndex].quantity = 0;
            inventoryManager.itemSlot[slotIndex].UpdateSlottedItem();

            // go to the next recursion level
            return MoveDataToPreviousSlot(slotIndex + 1); 
        }
    }
}