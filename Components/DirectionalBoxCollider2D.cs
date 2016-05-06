using UnityEngine;
using System.Collections;

[RequireComponent(typeof(BoxCollider2D))]
public class DirectionalBoxCollider2D : DirectionalComponent{

    public Vector2 leftOffset = new Vector2(0, 0);
    public Vector2 leftSize = new Vector2(1, 1);

    public Vector2 rightOffset = new Vector2(0, 0);
    public Vector2 rightSize = new Vector2(1, 1);

    public Vector2 upOffset = new Vector2(0, 0);
    public Vector2 upSize = new Vector2(1, 1);

    public Vector2 downOffset = new Vector2(0, 0);
    public Vector2 downSize = new Vector2(1, 1);

    private BoxCollider2D _boxCollider;

    public BoxCollider2D boxCollider
    {
        get
        {
            if (_boxCollider == null)
            {
                _boxCollider = GetComponent<BoxCollider2D>();
            }
            return _boxCollider;
        }
    }

    //Sets the collider to show the selected direction's current layout based on the currently saved values.
    [ContextMenu("Left Collider Preview")]
    protected override void SetLeft()
    {
        boxCollider.offset = leftOffset;
        boxCollider.size = leftSize;
    }

    [ContextMenu("Right Collider Preview")]
    protected override void SetRight()
    {
        boxCollider.offset = rightOffset;
        boxCollider.size = rightSize;
    }

    [ContextMenu("Up Collider Preview")]
    protected override void SetUp()
    {
        boxCollider.offset = upOffset;
        boxCollider.size = upSize;
    }

    [ContextMenu("Down Collider Preview")]
    protected override void SetDown()
    {
        boxCollider.offset = downOffset;
        boxCollider.size = downSize;
    }

    //Applies and stores collider variables for each direction based on current collider variables.
    [ContextMenu("Apply Left Postional Values From Collider")]
    void ApplyLeft()
    {
        leftOffset = boxCollider.offset;
        leftSize = boxCollider.size;
    }

    [ContextMenu("Apply Right Postional Values From Collider")]
    void ApplyRight()
    {
        rightOffset = boxCollider.offset;
        rightSize = boxCollider.size;
    }
    [ContextMenu("Apply Up Postional Values From Collider")]
    void ApplyUp()
    {
        upOffset = boxCollider.offset;
        upSize = boxCollider.size;
    }
    [ContextMenu("Apply Down Postional Values From Collider")]
    void ApplyDown()
    {
        downOffset = boxCollider.offset;
        downSize = boxCollider.size;
    }
    

}
