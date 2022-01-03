using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ItemGrid))]
public class GridInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    InventoryController inventoryController;
    ItemGrid itemGrid;

    private void Awake()
    {
        inventoryController = FindObjectOfType(typeof(InventoryController)) as InventoryController;
        itemGrid = GetComponent<ItemGrid>();
        //Debug.Log(gameObject.name + " " + transform.position);
    }

    //set current bag as contolling inventory
    public void OnPointerEnter(PointerEventData eventData)
    {
        //Debug.Log("in" + gameObject.name);
        inventoryController.SelectedItemGrid = itemGrid;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        //Debug.Log("Out" + gameObject.name);
        inventoryController.SelectedItemGrid = null;
    }
}
