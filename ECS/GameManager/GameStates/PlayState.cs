using System;
using System.Collections;
using Unity.Entities;

public struct PlayStateTag : IGameState { }


public class PlayState : GameState
{
    public override IEnumerator Enter()
    {
        GameManager.Instance.OnApplicationPauseEvent += OnApplicationPause;
        GameManager.Instance.OnApplicationFocusEvent += OnApplicationFocus;
        yield break;
    }


    public override IEnumerator Exit()
    {
        GameManager.Instance.OnApplicationPauseEvent -= OnApplicationPause;
        GameManager.Instance.OnApplicationFocusEvent -= OnApplicationFocus;
        yield break;
    }

    public override IEnumerator Update()
    {
        yield return null;
    }


    private void OnApplicationPause(bool paused)
    {
        if (paused) GameManager.Instance.ChangeState<PauseState>();
    }

    private void OnApplicationFocus(bool focused)
    {
        GameManager.Instance.ChangeState<PauseState>();
    }

    public override void EnableTag(Entity entity, EntityManager entityManager, bool enable) => 
        ToggleTag<PlayStateTag>(entity, entityManager, enable);
}