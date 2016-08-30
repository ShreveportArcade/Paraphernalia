using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class ListLayoutGroup : LayoutGroup {
   
    public delegate void OnRowUpdate(int row, int child);
    public event OnRowUpdate onRowUpdate = delegate {};

    [SerializeField] protected int _rows = 1;
    public int rows { 
        get { return _rows; } 
        set { SetProperty(ref _rows, value); } 
    }

    [SerializeField] protected float _height = 100;
    public float height { 
        get { return _height; } 
        set { SetProperty(ref _height, value); } 
    }

    [SerializeField] protected float _spacing = 5;
    public float spacing { 
        get { return _spacing; } 
        set { SetProperty(ref _spacing, value); } 
    }
    
    public int firstVisible {
        get {
            if (transform.parent == null) return 0;
            return (int)(rectTransform.anchoredPosition.y / height);
        }
    }
    public int visibleCount {
        get {
            if (transform.parent == null) return 1;
            RectTransform parent = transform.parent.GetComponent<RectTransform>();
            return Mathf.CeilToInt(parent.rect.height / (height + spacing)) + 1;
        }
    }
    
    public override void CalculateLayoutInputHorizontal () {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical () {
        float space = padding.vertical + (height + spacing) * rows;
        SetLayoutInputForAxis(space, space, -1, 1);
    }

    public override void SetLayoutVertical () {
        if (rectChildren.Count == 0) return;
        for (int i = firstVisible; i < Mathf.Min(firstVisible + visibleCount, rows); i++) {
            float y = padding.vertical + (height + spacing) * i;
            int child = i % rectChildren.Count;
            SetChildAlongAxis(rectChildren[child], 1, y, height);
            onRowUpdate(i, child);
        }
    }

    public override void SetLayoutHorizontal () {
        if (rectChildren.Count == 0) return;
        float width = rectTransform.rect.size.x;
        for (int i = 0; i < rectChildren.Count; i++) {
            SetChildAlongAxis(rectChildren[i], 0, padding.horizontal, width);
        }
    }
}