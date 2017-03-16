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
            return Mathf.CeilToInt(parent.rect.height / (height + spacing));
        }
    }

    private float unusedHeight {
        get {
            RectTransform rect = transform as RectTransform;
            float space = padding.vertical + height * rows + spacing * (rows - 1);
            float unused = rect.rect.height - space;
            return unused;
        }
    }
    
    public override void CalculateLayoutInputHorizontal () {
        base.CalculateLayoutInputHorizontal();
    }

    public override void CalculateLayoutInputVertical () {
        float space = padding.vertical + height * rows + spacing * (rows - 1);
        SetLayoutInputForAxis(space, space, -1, 1);
    }

    public override void SetLayoutVertical () {
        if (rectChildren.Count == 0) return;
        float offset = 0;
        if (childAlignment == TextAnchor.MiddleLeft || 
            childAlignment == TextAnchor.MiddleCenter ||
            childAlignment == TextAnchor.MiddleRight) {
            offset = unusedHeight * 0.5f;
        }
        else if (childAlignment == TextAnchor.LowerLeft || 
            childAlignment == TextAnchor.LowerCenter ||
            childAlignment == TextAnchor.LowerRight) {
            offset = unusedHeight;
        }
        for (int i = Mathf.Max(0, firstVisible); i < Mathf.Min(firstVisible + visibleCount, rows); i++) {
            float y = offset + padding.vertical + (height + spacing) * i;
            int child = i % rectChildren.Count;
            SetChildAlongAxis(rectChildren[child], 1, y, height);
            onRowUpdate(i, child);
        }
    }

    public override void SetLayoutHorizontal () {
        if (rectChildren.Count == 0) return;
        float width = rectTransform.rect.size.x - padding.horizontal;
        for (int i = 0; i < rectChildren.Count; i++) {
            SetChildAlongAxis(rectChildren[i], 0, padding.left, width);
        }
    }
}