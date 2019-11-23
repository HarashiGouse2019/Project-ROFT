using UnityEngine;

public static class ClickEvent
{
    public static GameObject targetKey;
    public static bool ClickReceived()
    {
        bool leftClick = Input.GetMouseButtonDown(0);
        bool rightClick = Input.GetMouseButtonDown(1);

        CheckNextKey();

        return (leftClick || rightClick);
    }

    public static void CheckNextKey()
    {
        if (targetKey != null)
            ClosestObjectClass.targetKey = targetKey;
    }
}
