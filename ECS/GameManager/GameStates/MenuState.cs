using System.Collections;
using Unity.Entities;

public struct MenuStateTag : IGameState { }

public class MenuState : GameState
{
    public override IEnumerator Enter()
    {
        MainMenuManager.Instance.menuPanel.Show();
        yield break;
    }

    public override IEnumerator Exit()
    {
        MainMenuManager.Instance.menuPanel.Hide();
        yield break;
    }

    public override void EnableTag(Entity entity, EntityManager entityManager, bool enable) => 
        ToggleTag<MenuStateTag>(entity, entityManager, enable);
}


