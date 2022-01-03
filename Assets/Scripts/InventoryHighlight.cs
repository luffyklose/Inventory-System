using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryHighlight : MonoBehaviour
{
    [SerializeField] RectTransform highlighter;
    [SerializeField] private Sprite GreenHighlith;
    [SerializeField] private Sprite RedHighlight;

    public void Show(bool b) 
    {
        highlighter.gameObject.SetActive(b);
    }

    //Adjust highlight's size based on item's shape and single grid's size
    public void SetSize(InventoryItem targetItem) 
    {
        Vector2 size = new Vector2();
        size.x = targetItem.WIDTH * ItemGrid.tileSizeWidth;
        size.y = targetItem.HEIGHT * ItemGrid.tileSizeHeight;
        highlighter.sizeDelta = size;
    }

    //Calculate highlight's position of unselected item.
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem)
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(
            targetItem,
            targetItem.onGridPositionX,
            targetItem.onGridPositionY
            );

        highlighter.localPosition = pos;
    }

    public void SetParent(ItemGrid targetGrid)
    {
        if (targetGrid == null) { return; }
        highlighter.SetParent(targetGrid.GetComponent<RectTransform>());
    }

    //Adjust highlight's position into proper grid
    public void SetPosition(ItemGrid targetGrid, InventoryItem targetItem, int posX, int posY) 
    {
        Vector2 pos = targetGrid.CalculatePositionOnGrid(
            targetItem,
            posX,
            posY
            );

        highlighter.localPosition = pos;
    }

    //Cannot use now.
    public void ChangeHighlightSprite(string color)
    {
        switch (color)
        {
            case"red":
                highlighter.GetComponent<Image>().sprite = RedHighlight;
                break;
            case"green":
                highlighter.GetComponent<Image>().sprite = GreenHighlith;
                break;
            default: break;
        }
    }
}
