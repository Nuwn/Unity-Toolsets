## On Screen Joystick for UI Toolkit

How to use.

In your UI builder go to Project -> Search Joystick -> Drag the OnScreenJoystick to the hierarchy.

Name your component <br />
Set up Radius, this will also control the overall size. <br />
Set up deadZone.

Style OnScreenJoystick for the Radius. <br />
Add a new VisualElement to OnScreenJoystick, this will be your graphical representation of the thumbstick.

<br />
Example Usage

```c#
public class OnScreenInputController : MonoBehaviour
{
    OnScreenJoystick moveStick;

    private void OnEnable()
    {
        UIDocument uiDoc = GetComponent<UIDocument>();
        moveStick = uiDoc.rootVisualElement.Q<OnScreenJoystick>("JoystickMove");
    }

    private void Update()
    {
        if(moveStick.CurrentInteraction == JoystickInteraction.Moving)
            Log(moveStick.Input);
    }
}
```