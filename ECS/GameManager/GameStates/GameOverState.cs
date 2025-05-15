using System.Collections;
using Unity.Entities;

public struct GameOverStateTag : IGameState { }

public class GameOverState : GameState
{
    public override IEnumerator Enter()
    {
        yield return base.Enter();
    }

    public override IEnumerator Exit()
    {
        yield return base.Exit();
    }

    public override IEnumerator Update()
    {
        yield return base.Update();
    }

    public override void EnableTag(Entity entity, EntityManager entityManager, bool enable) =>
        ToggleTag<GameOverStateTag>(entity, entityManager, enable);
}