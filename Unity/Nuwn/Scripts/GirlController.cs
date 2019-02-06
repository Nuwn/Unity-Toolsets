using UnityEngine;
using Nuwn.Essentials;
using Nuwn.Extensions;

public class GirlController : MonoBehaviour
{
    protected Animator anim;

    private void Start()
    {
        anim = GetComponentInChildren<Animator>();
    }
}
