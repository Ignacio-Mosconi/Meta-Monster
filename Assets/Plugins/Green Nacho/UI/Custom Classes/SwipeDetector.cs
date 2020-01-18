using System;
using UnityEngine;

public enum SwipeType
{
    Horizontal,
    Vertical
}

public enum SwipeState
{
    None,
    PositiveSwipe,
    NegativeSwipe
}

[System.Serializable]
public class SwipeDetector
{
    [SerializeField] SwipeType swipeType = SwipeType.Horizontal;
    [SerializeField, Range(0f, 1000f)] float swipeActivationDelta = 200f;

    public Action<SwipeState> OnSwipeFinished { get; set; }

    Vector2 swipeStartPosition;
    Vector2 swipeCurrentPosition;

    bool HasSwipped()
    {
        bool hasSwipped = false;
        
        Vector2 swipeDiff = (swipeCurrentPosition - swipeStartPosition);
        float horSwipeDelta = Mathf.Abs(swipeDiff.x);
        float verSwipeDelta = Mathf.Abs(swipeDiff.y);

        if (((swipeType == SwipeType.Horizontal && horSwipeDelta >verSwipeDelta) || 
            (swipeType == SwipeType.Vertical && verSwipeDelta > horSwipeDelta)) && 
            swipeDiff.sqrMagnitude > swipeActivationDelta * swipeActivationDelta)
            hasSwipped = true;

        return hasSwipped;
    }

    SwipeState DetectSwipe()
    {
        SwipeState swipeState = SwipeState.None;

        if (HasSwipped())
        {
            if (swipeType == SwipeType.Horizontal)
                swipeState = (swipeCurrentPosition.x > swipeStartPosition.x) ? SwipeState.PositiveSwipe : SwipeState.NegativeSwipe;
            else
                swipeState = (swipeCurrentPosition.y > swipeStartPosition.y) ? SwipeState.PositiveSwipe : SwipeState.NegativeSwipe;
        }

        return swipeState;
    }

    void ResetSwipe()
    {
        swipeStartPosition = swipeCurrentPosition = Vector2.zero;
    }

    public void UpdateSwipe()
    {
        if (Input.touchCount != 1)
            return;

        Touch touch = Input.GetTouch(0);

        switch (touch.phase)
        {
            case TouchPhase.Began:
                swipeStartPosition = touch.position;
                break;

            case TouchPhase.Moved:
                swipeCurrentPosition = touch.position;
                break;

            case TouchPhase.Ended:
                SwipeState swipeState = DetectSwipe();
                if (swipeState != SwipeState.None)
                {
                    ResetSwipe();
                    OnSwipeFinished.Invoke(swipeState);
                }
                break;
        }
    }
}