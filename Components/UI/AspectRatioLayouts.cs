using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class AspectRatioLayouts : UIBehaviour, ILayoutSelfController {

    [System.Serializable]
    public struct AspectRatioLayout {
        public float aspect;
        public Vector2 anchorMin;
        public Vector2 anchorMax;
        public Vector2 anchoredPosition;
        public Vector2 pivot;
        public Vector2 sizeDelta;

        public AspectRatioLayout (float aspect, Vector2 anchorMin, Vector2 anchorMax) {
            this.aspect = aspect;
            this.anchorMin = anchorMin;
            this.anchorMax = anchorMax;
            this.pivot = Vector2.zero;
            this.anchoredPosition = Vector2.zero;
            this.sizeDelta = Vector2.zero;
        }
    }

    public AspectRatioLayout[] anchors = new AspectRatioLayout[] {
        new AspectRatioLayout(0.5f, Vector2.zero, Vector2.one),
        new AspectRatioLayout(2, Vector2.zero, Vector2.one)
    };

    private RectTransform _rectTransform;
    private RectTransform rectTransform {
        get {
            if (_rectTransform == null) {
                _rectTransform = GetComponent<RectTransform>();
            }
            return _rectTransform;
        }
    }

    #if UNITY_EDITOR
    protected override void OnValidate() {
        UpdateLayouts();
    }
    #endif

    protected override void OnRectTransformDimensionsChange() {
        UpdateLayouts();
    }

    public void SetLayoutHorizontal() {
        UpdateLayouts();
    }

    public void SetLayoutVertical() {
        UpdateLayouts();
    }

    void UpdateLayouts () {
        if (transform.parent == null) {
            Debug.LogWarning("AspectRatioLayouts requires a parent.");
            return;
        }

        if (anchors.Length < 2) {
            Debug.LogWarning("AspectRatioLayouts requires at least two reference anchors.");
        }
        
        RectTransform parent = transform.parent as RectTransform;
        Vector2 parentSize = parent.rect.size;
        float aspect = parentSize.x / parentSize.y;

        AspectRatioLayout closest = anchors[0];
        float closestDist = Mathf.Abs(aspect - closest.aspect);
        for (int i = 1; i < anchors.Length; i++) {
            float dist = Mathf.Abs(anchors[i].aspect - aspect);
            if (dist < closestDist) {
                closest = anchors[i];
                closestDist = dist;
            }
        }
        rectTransform.anchorMin = closest.anchorMin;
        rectTransform.anchorMax = closest.anchorMax;
        rectTransform.anchoredPosition = closest.anchoredPosition;
        rectTransform.pivot = closest.pivot;
        rectTransform.sizeDelta = closest.sizeDelta;
    }
}
