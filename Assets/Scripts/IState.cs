using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void Enter();
    public void Update(float delta);
    public void FixedUpdate();
    public void Exit();
}
