using System.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UIElements;

public struct StartStateTag : IGameState { }

public class StartState : GameState
{
    VisualElement countdown;
    Label text;

    int time = 3;

    public override IEnumerator Enter()
    {
        countdown = MainMenuManager.Instance.Countdown;
        text = countdown.Q<Label>("CountdownLabel");
        text.text = time.ToString("0");

        EnableCountdown(true);
        yield break;
    }

    public override IEnumerator Exit()
    {
        EnableCountdown(false);
        yield break;
    }

    private void EnableCountdown(bool v)
    {
        countdown.visible = v;
        countdown.style.display = v ? DisplayStyle.Flex : DisplayStyle.None;
    }

    public override IEnumerator Update()
    {
        for (int i = time; i > 0; i--)
        {
            text.text = i.ToString();
            yield return new WaitForSecondsRealtime(1f);
        }

        text.text = "Go!";
        yield return new WaitForSecondsRealtime(1f);
        GameManager.Instance.ChangeState<PlayState>();
    }

    public override void EnableTag(Entity entity, EntityManager entityManager, bool enable) => 
        ToggleTag<StartStateTag>(entity, entityManager, enable);
}
