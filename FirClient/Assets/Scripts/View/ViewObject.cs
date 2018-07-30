using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FirClient.View
{
    public enum ViewType
    {
        Tank,
        Animal,
        Bullet,
        Effect
    }

    public class ViewObject : GameBehaviour
    {
        public ViewType viewType;

        private INPCView npcView;
        private IObjectView objView;

        protected override void OnStart()
        {
            if (npcView != null)
            {
                npcView.OnStart();
            }
            if (objView != null)
            {
                objView.OnStart();
            }
            base.OnStart();
        }

        protected override void OnUpdate()
        {
            if (npcView != null)
            {
                npcView.OnUpdate();
            }
            if (objView != null)
            {
                objView.OnUpdate();
            }
            base.OnUpdate();
        }

        protected override void OnTriggerCollide2D(Collider2D coll)
        {
            if (npcView != null)
            {
                npcView.OnTriggerEnter2D(coll);
            }
            if (objView != null)
            {
                objView.OnTriggerEnter2D(coll);
            }
            base.OnTriggerCollide2D(coll);
        }

        protected override void OnDestroyMe()
        {
            if (npcView != null)
            {
                npcView.OnDestroy();
            }
            if (objView != null)
            {
                objView.OnDispose();
            }
            base.OnDestroyMe();
        }

        public void BindView(INPCView view)
        {
            npcView = view;
            Type type = view.GetType();
            if (type == typeof(TankView))
            {
                viewType = ViewType.Tank;
            }
            else if (type == typeof(AnimalView))
            {
                viewType = ViewType.Animal; 
            }
            var _npcView = view as NPCView;
            if (_npcView != null)
            {
                _npcView.viewObject = this;
                _npcView.transform = transform;
                _npcView.gameObject = gameObject;
            }
        }

        public void BindView(IObjectView view)
        {
            objView = view;
            var type = view.GetType();
            if (type == typeof(EffectView))
            {
                viewType = ViewType.Effect;
            }
            else if (type == typeof(BulletView))
            {
                viewType = ViewType.Bullet;
            }
            var _objView = objView as ObjectView;
            _objView.viewObject = this;
            _objView.transform = transform;
            _objView.gameObject = gameObject;
        }
    }
}

