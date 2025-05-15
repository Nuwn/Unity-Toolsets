using System.Collections;
using Unity.Entities;

public struct PauseStateTag : IGameState { }


internal class PauseState : GameState
{
    public override IEnumerator Enter()
    {
        MainMenuManager.Instance.pausePanel.Show();
        yield break;
    }

    public override IEnumerator Exit()
    {
        MainMenuManager.Instance.pausePanel.Hide();
        yield break;
    }

    public override IEnumerator Update()
    {
        yield return base.Update();
    }

    public override void EnableTag(Entity entity, EntityManager entityManager, bool enable) => 
        ToggleTag<PauseStateTag>(entity, entityManager, enable);
}