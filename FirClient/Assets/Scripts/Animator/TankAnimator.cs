using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FirClient.Controller;
using DG.Tweening;

namespace FirClient.Animator
{
    public class TankAnimator : AnimController
    {
        private float canon_y;
        private Transform canon;

        protected override void OnAwake()
        {
            base.OnAwake();
            canon = transform.Find("Canon");
            canon_y = canon.localPosition.y;
        }

        public override void OnAttack()
        {
            base.OnAttack();
            this.PlayTankFire();
        }

        void PlayTankFire()
        {
            if (canon != null)
            {
                canon.DOLocalMoveY(-0.1f, 0.15f).OnComplete(delegate()
                {
                    canon.DOLocalMoveY(canon_y, 0.15f);
                });
            }
        }
    }
}

