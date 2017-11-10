using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
[RequireComponent(typeof(RectTransform))]
public class AspectRatioAnchors : UIBehaviour, ILayoutSelfController {

    [System.Serializable]
    public struct AspectRatioAnchor {
        public float aspect;
        public Vector2 min;
        public Vector2 max;

        public AspectRatioAnchor (float aspect, Vector2 min, Vector2 max) {
            this.aspect = aspect;
            this.min = min;
            this.max = max;
        }
    }

    public AspectRatioAnchor[] anchors = new AspectRatioAnchor[] {
        new AspectRatioAnchor(0.5f, Vector2.zero, Vector2.one),
        new AspectRatioAnchor(2, Vector2.zero, Vector2.one)
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
        // set anchors from scene view
        UpdateAnchors();
    }
    #endif

    protected override void OnRectTransformDimensionsChange() {
        // set anchors from scene inspector
        UpdateAnchors();
    }

    public void SetLayoutHorizontal() {
        UpdateAnchors();
    }

    public void SetLayoutVertical() {
        UpdateAnchors();
    }

    void UpdateAnchors () {
        if (transform.parent == null) {
            Debug.LogWarning("AspectRatioAnchors requires a parent.");
            return;
        }

        if (anchors.Length < 2) {
            Debug.LogWarning("AspectRatioAnchors requires at least two reference anchors.");
        }
        
        RectTransform parent = transform.parent as RectTransform;
        Vector2 parentSize = parent.rect.size;
        float aspect = parentSize.x / parentSize.y;

        AspectRatioAnchor closest = anchors[0];
        float closestDist = Mathf.Abs(aspect - closest.aspect);
        for (int i = 1; i < anchors.Length; i++) {
            float dist = Mathf.Abs(anchors[i].aspect - aspect);
            if (dist < closestDist) {
                closest = anchors[i];
                closestDist = dist;
            }
        }
        rectTransform.anchorMin = closest.min;
        rectTransform.anchorMax = closest.max;
    }
}
