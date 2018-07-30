using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AnimKey
{
    Idle,
    Attack,
    Death,
    Walk,
    Run,
}

namespace FirClient.Controller
{
    public class AnimController : GameBehaviour
    {
        private AnimKey animKey = AnimKey.Idle;

        protected override void OnAwake()
        {
        }

        public void SetPlay(AnimKey key)
        {
            animKey = key;
            switch (key)
            {
                case AnimKey.Idle:
                OnIdle();
                break;
                case AnimKey.Attack:
                OnAttack();
                break;
                case AnimKey.Death:
                OnDeath();
                break;
                case AnimKey.Walk:
                OnWalk();
                break;
                case AnimKey.Run:
                OnRun();
                break;
            }
        }

        public virtual void OnIdle() { }
        public virtual void OnAttack() { }
        public virtual void OnDeath() { }
        public virtual void OnWalk() { }
        public virtual void OnRun() { }
    }

}
