using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGrid : MonoBehaviour
{
    public const float tileSizeWidth = 96;
    public const float tileSizeHeight = 96;

    InventoryItem[,] inventoryItemSlot;

    RectTransform rectTransform;

    [SerializeField] int gridSizeWidth = 5;
    [SerializeField] int gridSizeHeight = 5;

    //Generate grid array.
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        Init(gridSizeWidth, gridSizeHeight);
    }

    //test code.
    private void Update()
    {
        //Vector2 pos = (Vector2)Input.mousePosition;
        //Debug.Log(GetTileGridPosition(pos));
    }

    //Pick up selected item. Reset the grid's state which it occupied.
    public InventoryItem PickUpItem(int x, int y)
    {
        InventoryItem toReturn = inventoryItemSlot[x, y];

        if (toReturn == null) { return null; }

        CleanGridReference(toReturn);

        return toReturn;
    }

    //Reset the grid's state which an item occupied.
    private void CleanGridReference(InventoryItem item)
    {
        for (int ix = 0; ix < item.WIDTH; ix++)
        {
            for (int iy = 0; iy < item.HEIGHT; iy++)
            {
                inventoryItemSlot[item.onGridPositionX + ix, item.onGridPositionY + iy] = null;
            }
        }
    }

    //Generate an array to store grids. All of the grids don't exist actually, the use of them is setting item's position based on grid's position.
    private void Init(int width, int height)
    {
        inventoryItemSlot = new InventoryItem[width, height];
        Vector2 size = new Vector2(width * tileSizeWidth, height * tileSizeHeight);
        rectTransform.sizeDelta = size;
    }

    internal InventoryItem GetItem(int x, int y)
    {
        return inventoryItemSlot[x, y];
    }

    //Calculate the relative position of mouse position and get which grid it's in.
    Vector2 positionOnTheGrid = new Vector2();
    Vector2Int tileGridPosition = new Vector2Int();
    public Vector2Int GetTileGridPosition(Vector2 mousePosition) 
    {
        positionOnTheGrid.x = mousePosition.x - rectTransform.position.x;
        positionOnTheGrid.y = rectTransform.position.y - mousePosition.y;

        tileGridPosition.x = (int)(positionOnTheGrid.x / tileSizeWidth);
        tileGridPosition.y = (int)(positionOnTheGrid.y / tileSizeHeight);

        //Debug.Log("x:" + positionOnTheGrid.x + "/" + tileSizeWidth + "=" + tileGridPosition.x);
        //Debug.Log("y:" + positionOnTheGrid.y + "/" + tileSizeWidth + "=" + tileGridPosition.y);

        return tileGridPosition;
    }

    //Find a proper grid to place an item. Based on item's shape, traverse all the grid array, if a grid and other calculated grids are empty, return this grid.
    public Vector2Int? FindSpaceForObject(InventoryItem itemToInsert)
    {
        int height = gridSizeHeight - itemToInsert.HEIGHT + 1;
        int width = gridSizeWidth - itemToInsert.WIDTH + 1;

        for (int y = 0; y < height; y++) 
        {
            for (int x = 0; x < width; x++)
            {
                if (CheckAvailableSpace(x, y, itemToInsert.WIDTH, itemToInsert.HEIGHT) == true) 
                {
                    return new Vector2Int(x, y);
                }
            }
        }

        return null;
    }

    //Useless now, need to be modified.
    public bool CheckOverlap(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            return true;
        }

        return false;
    }

    //Check if an item can be placed in a specific grid.
    public bool TryPlaceItem(InventoryItem inventoryItem, int posX, int posY, ref InventoryItem overlapItem)
    {
        //Check if this item will out of border
        if (BoundryCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT) == false)
        {
            return false;
        }

        //Check if this item overlaps other item
        if (OverlapCheck(posX, posY, inventoryItem.WIDTH, inventoryItem.HEIGHT, ref overlapItem) == false)
        {
            overlapItem = null;
            return false;
        }

        //Clear overlapped item's grid
        if (overlapItem != null)
        {
            CleanGridReference(overlapItem);
        }

        PlaceItem(inventoryItem, posX, posY);

        return true;
    }

    //Place an item to a specific grid.
    public void PlaceItem(InventoryItem inventoryItem, int posX, int posY)
    {
        RectTransform rectTransform = inventoryItem.GetComponent<RectTransform>();
        rectTransform.SetParent(this.rectTransform);

        for (int x = 0; x < inventoryItem.WIDTH; x++)
        {
            for (int y = 0; y < inventoryItem.HEIGHT; y++)
            {
                inventoryItemSlot[posX + x, posY + y] = inventoryItem;
            }
        }

        inventoryItem.onGridPositionX = posX;
        inventoryItem.onGridPositionY = posY;
        Vector2 position = CalculatePositionOnGrid(inventoryItem, posX, posY);

        rectTransform.localPosition = position;
    }

    public Vector2 CalculatePositionOnGrid(InventoryItem inventoryItem, int posX, int posY)
    {
        Vector2 position = new Vector2();
        position.x = posX * tileSizeWidth + tileSizeWidth * inventoryItem.WIDTH / 2;
        position.y = -(posY * tileSizeHeight + tileSizeHeight * inventoryItem.HEIGHT / 2);
        return position;
    }

    //Check if an item overlap other item. Update overlap item if there's only one overlapped item. Return false if there are more than one.
    private bool OverlapCheck(int posX, int posY, int width, int height, ref InventoryItem overlapItem)
    {
        for (int x = 0; x < width; x++) 
        {
            for (int y = 0; y < height; y++) 
            {
                if (inventoryItemSlot[posX + x, posY + y] != null) 
                {
                    if (overlapItem == null)
                    {
                        overlapItem = inventoryItemSlot[posX + x, posY + y];
                    }
                    else {
                        if (overlapItem != inventoryItemSlot[posX + x, posY + y]) 
                        {
                            return false;
                        }
                    }
                }
            }
        }

        return true;
    }

    //Check if an item can place in a specific grid based on the item's shape.
    private bool CheckAvailableSpace(int posX, int posY, int width, int height)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (inventoryItemSlot[posX + x, posY + y] != null)
                {

                    return false;

                }
            }
        }

        return true;
    }

    //Check if a grid is valid.
    public bool PositionCheck(int posX, int posY) 
    {
        if (posX < 0 || posY < 0) 
        {
            return false;
        }

        if (posX >= gridSizeWidth || posY >= gridSizeHeight) 
        {
            return false;
        }

        return true;
    }

    //Check weather an item will be out of bounds if placed in an specific grid.
    public bool BoundryCheck(int posX, int posY, int width, int height) 
    {
        if (PositionCheck(posX, posY) == false) { return false; }

        posX += width-1;
        posY += height-1;

        if (PositionCheck(posX, posY) == false) { return false; }

        return true;
    }
}
