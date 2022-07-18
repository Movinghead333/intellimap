using UnityEngine;

public class IntellimapInput {
    public static bool LeftMouseButton() {
        return Event.current.button == 0;
    }

    public static bool MouseDown() {
        return Event.current.type == EventType.MouseDown;
    }

    public static bool MouseUp() {
        return Event.current.type == EventType.MouseUp;
    }

    public static bool MouseDrag() {
        return Event.current.type == EventType.MouseDrag;
    }

    public static bool MouseMove() {
        return Event.current.type == EventType.MouseMove;
    }

    public static Vector2 MouseScroll() {
        if (Event.current.isScrollWheel) {
            return Event.current.delta;        
        }
        else {
            return Vector2.zero;
        }
    }

    public static bool CtrlHeld() {
        return Event.current.control;
    }
}
