using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryController : MonoBehaviour
{
    [HideInInspector]
    private ItemGrid selectedItemGrid;

    public ItemGrid SelectedItemGrid { 
        get => selectedItemGrid;
        set {
            selectedItemGrid = value;
            inventoryHighlight.SetParent(value);
        }
    }

    InventoryItem selectedItem;
    InventoryItem overlapItem;
    RectTransform rectTransform;
    private Vector2Int PreviousPos;

    [SerializeField] List<ItemData> items;
    [SerializeField] GameObject itemPrefab;
    [SerializeField] Transform canvasTransform;

    InventoryHighlight inventoryHighlight;

    private void Awake()
    {
        inventoryHighlight = GetComponent<InventoryHighlight>();
    }

    //Detect input. Generate new item; pick up and place item.
    //Move item if selecting any.
    private void Update()
    {
        ItemIconDrag();

        if (Input.GetKeyDown(KeyCode.Q)) 
        {
            if (selectedItem == null) 
            {
                CreateRandomItem();
            }
            
        }

        if (Input.GetKeyDown(KeyCode.Z)) 
        {
            InsertRandomItem();
        }

        if (selectedItemGrid == null) 
        {
            inventoryHighlight.Show(false);
            return; 
        }

        HandleHighlight();

        if (Input.GetMouseButtonDown(0))
        {
            LeftMouseButtonPress();
        }
    }

    //Generate a new item and place it into proper position.
    private void InsertRandomItem()
    {
        if (selectedItemGrid == null) { return; }

        CreateRandomItem();
        InventoryItem itemToInsert = selectedItem;
        selectedItem = null;
        InsertItem(itemToInsert);
    }

    //Based on item's shape, choose a grid to place this item. 
    private void InsertItem(InventoryItem itemToInsert)
    {
        Vector2Int? posOnGrid = selectedItemGrid.FindSpaceForObject(itemToInsert);

        if (posOnGrid == null)
        {
            itemToInsert.gameObject.SetActive(false);
            return;
        }

        selectedItemGrid.PlaceItem(itemToInsert, posOnGrid.Value.x, posOnGrid.Value.y);
    }

    Vector2Int oldPosition;
    InventoryItem itemToHighlight;
    
    //Handle the highlight based on current mouse position.
    private void HandleHighlight()
    {
        //Get which gird the mouse is. If mouse didn't move to another gird, then return.
        Vector2Int positionOnGrid = GetTileGridPosition();
        if (oldPosition == positionOnGrid) { return; }
        
        oldPosition = positionOnGrid;
        if (selectedItem == null)
        {
            //If the player doesn't select any item, highlight that item mouse point to. If mouse point to nothing, do nothing.
            //Debug.Log("get item: " + positionOnGrid.x + " " + positionOnGrid.y);
            itemToHighlight = selectedItemGrid.GetItem(positionOnGrid.x, positionOnGrid.y);

            if (itemToHighlight != null)
            {
                inventoryHighlight.Show(true);
                inventoryHighlight.SetSize(itemToHighlight);
                inventoryHighlight.SetPosition(selectedItemGrid, itemToHighlight);
            }
            else {
                inventoryHighlight.Show(false);
            }
        }
        else {
            //Add highlight to the gird below the item
            inventoryHighlight.Show(selectedItemGrid.BoundryCheck(
                positionOnGrid.x, 
                positionOnGrid.y, 
                selectedItem.WIDTH,
                selectedItem.HEIGHT)
                );

            inventoryHighlight.SetSize(selectedItem);
            inventoryHighlight.SetPosition(selectedItemGrid, selectedItem, positionOnGrid.x, positionOnGrid.y);
        }
    }

    //Choose an item from list randomly
    private void CreateRandomItem()
    {
        InventoryItem inventoryItem = Instantiate(itemPrefab).GetComponent<InventoryItem>();
        selectedItem = inventoryItem;

        rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(canvasTransform);
        rectTransform.SetAsLastSibling();

        int selectedItemID = UnityEngine.Random.Range(0, items.Count);
        inventoryItem.Set(items[selectedItemID]);
    }

    //Get the mouse position. Pick up an item if selecting nothing, place one if selecting any.
    private void LeftMouseButtonPress()
    {
        Vector2Int tileGridPosition = GetTileGridPosition();

        if (selectedItem == null)
        {
            PickUpItem(tileGridPosition);
        }
        else
        {
            PlaceItem(tileGridPosition);
        }
    }

    //Get relative position of mouse's current position
    private Vector2Int GetTileGridPosition()
    {
        Vector2 position = Input.mousePosition;

        if (selectedItem != null)
        {
            position.x -= (selectedItem.WIDTH - 1) * ItemGrid.tileSizeWidth / 2;
            position.y += (selectedItem.HEIGHT - 1) * ItemGrid.tileSizeHeight / 2;
        }

        return selectedItemGrid.GetTileGridPosition(position);
    }

    //Place the item to the grid mouse point to. Swap with orginal item if there's only one item overlaping.
    private void PlaceItem(Vector2Int tileGridPosition)
    {
        /*Vector2Int temp = selectedItemGrid.GetTileGridPosition(Input.mousePosition);
        Debug.Log(temp.x + " " + temp.y);
        Debug.Log(selectedItemGrid.PositionCheck(temp.x, temp.y));
        
        if (!selectedItemGrid.PositionCheck(temp.x, temp.y))
        {
            Debug.Log("rengdiao");
            PlaceItem(PreviousPos);
            return;
        }*/
        bool complete = selectedItemGrid.TryPlaceItem(selectedItem, tileGridPosition.x, tileGridPosition.y, ref overlapItem);
        if (complete) 
        {
            selectedItem = null;
            if (overlapItem != null) 
            {
                selectedItem = overlapItem;
                overlapItem = null;
                rectTransform = selectedItem.GetComponent<RectTransform>();
                rectTransform.SetAsLastSibling();
            }
        }
    }

    //Pick an item if mouse click on any.
    private void PickUpItem(Vector2Int tileGridPosition)
    {
        selectedItem = selectedItemGrid.PickUpItem(tileGridPosition.x, tileGridPosition.y);
        if (selectedItem != null)
        {
            rectTransform = selectedItem.GetComponent<RectTransform>();
            rectTransform.SetParent(canvasTransform);
            //PreviousPos = new Vector2Int(selectedItem.onGridPositionX, selectedItem.onGridPositionY);
        }
    }

    //Move selected item's position following mouse's position.
    private void ItemIconDrag()
    {
        if (selectedItem != null)
        {
            rectTransform.position = Input.mousePosition;
            rectTransform.SetAsLastSibling();
            /*if (selectedItemGrid.CheckOverlap(selectedItem, GetTileGridPosition().x, GetTileGridPosition().y,
                ref overlapItem))
            {
                inventoryHighlight.ChangeHighlightSprite("red");
            }
            else
            {
                inventoryHighlight.ChangeHighlightSprite("green");
            }*/
        }
    }
}
