using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class SwitchableHorizontalOrVerticalLayoutGroup : HorizontalOrVerticalLayoutGroup {

	public bool isVertical = true;

    public override void CalculateLayoutInputHorizontal() {
        base.CalculateLayoutInputHorizontal();
        CalcAlongAxis(0, isVertical);
    }

    public override void CalculateLayoutInputVertical() {
        CalcAlongAxis(1, isVertical);
    }

    public override void SetLayoutHorizontal() {
        SetChildrenAlongAxis(0, isVertical);
    }

    public override void SetLayoutVertical() {
        SetChildrenAlongAxis(1, isVertical);
    }
}
