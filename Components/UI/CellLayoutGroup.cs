using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CellLayoutGroup : LayoutGroup {
   
    [SerializeField] protected int _rows = 1;
    public int rows { 
        get { return _rows; } 
        set { SetProperty(ref _rows, value); } 
    }

    [SerializeField] protected int _columns = 1;
    public int columns { 
        get { return _columns; } 
        set { SetProperty(ref _columns, value); } 
    }

    [SerializeField] protected Vector2 _spacing = Vector2.zero;
    public Vector2 spacing { 
        get { return _spacing; } 
        set { SetProperty(ref _spacing, value); } 
    }
    
    public override void CalculateLayoutInputHorizontal () {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical () {
    }

    public override void SetLayoutHorizontal () {
        if (rectChildren.Count == 0 || columns == 0) return;

        float width = (rectTransform.rect.width - padding.horizontal - spacing.x * (columns - 1)) / (float)columns;
        for (int i = 0; i < rectChildren.Count; i++) {
            float x = padding.left + (width + spacing.x) * (i % columns);
            CellLayoutElement cell = rectChildren[i].gameObject.GetComponent<CellLayoutElement>();
            if (cell == null) SetChildAlongAxis(rectChildren[i], 0, x, width);
            else SetChildAlongAxis(rectChildren[i], 0, x + width * cell.offset.x, width * cell.sizeScale.x);
        }        
    }

    public override void SetLayoutVertical () {
        if (rectChildren.Count == 0 || rows == 0) return;

        float height = (rectTransform.rect.height - padding.vertical - spacing.y * (rows - 1)) / (float)rows;
        for (int i = 0; i < rectChildren.Count; i++) {
            float y = padding.top + (height + spacing.y) * (i / columns);
            CellLayoutElement cell = rectChildren[i].gameObject.GetComponent<CellLayoutElement>();
            if (cell == null) SetChildAlongAxis(rectChildren[i], 1, y, height);
            else SetChildAlongAxis(rectChildren[i], 1, y + height * cell.offset.y, height * cell.sizeScale.y);
        }
    }
}