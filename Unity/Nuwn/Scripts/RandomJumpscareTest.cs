using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomJumpscareTest : JumpScare
{
    // needed stuff

    public override void Use()
    {
        SpawnJill();
    }
    public override void OnEnable()
    {
        Enabled();
    }


    // edit here
    public GameObject jill;

    private void Enabled()
    {

    }
    public void SpawnJill()
    {
        jill.SetActive(true);
        StartCoroutine(Kill());
    }
    IEnumerator Kill()
    {
        yield return new WaitForSeconds(1);
        PlayerStatus.Instance.Kill(PlayerStatus.Instance.testKill);
    }
}
