using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public class DragDrop : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 clickPos;
    private int[,] grid = {
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 1, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 1, 1, 2, 2, 1, 2, 2, 2, 1, 1, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 5, 0, 0},
        {0, 0, 0, 0, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0},
        {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}}; // the grid controll the possible place of the gear handel
    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData) // called every start of the drag
    {
        clickPos = rectTransform.anchoredPosition - eventData.position; // remember the stating drag position 
        canvasGroup.alpha = 0.6f;
        canvasGroup.blocksRaycasts = false;
    }
    public void OnDrag(PointerEventData eventData)
    {
        Vector2 diraction = (clickPos - (rectTransform.anchoredPosition - eventData.position)); // setting the diraction the gear shall move
        Vector2 pos2 = rectTransform.anchoredPosition + diraction/8;

        
        int gridX = (int)(pos2.x)/2 + 10;
        int gridY = (int)(pos2.y)/2 + 10;
        if (gridX > 0 && gridX < 19 && gridY > 0 && gridY < 22)
            if (grid[gridY, gridX] == 1)
                rectTransform.anchoredPosition = pos2;
            else
            Debug.Log("out of range");
        clickPos = rectTransform.anchoredPosition - eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData) // called every end of the drag
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }
}
