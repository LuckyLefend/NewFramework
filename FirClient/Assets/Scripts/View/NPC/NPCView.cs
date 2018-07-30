using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System;
using System.Collections;
using FirClient.Manager;
using LiteNetLib.Utils;
using FirClient.Network;
using FirClient.Animator;

namespace FirClient.View
{
    public class NPCView : NetworkObject, INPCView
    {
        public NPCData NpcData { get; set; }
        public Transform transform;
        public GameObject gameObject;
        public ViewObject viewObject;
        public GameObject roleObject;
        public Rigidbody2D rigidbody;
        public TankAnimator animObject;

        private Vector3 _position;
        private Quaternion _rotation;
        private byte[] _dirtyFields = new byte[1];
        private byte[] _readDirtyFlags = new byte[1];

        public event FieldEvent<Vector3> positionChanged;
        public InterpolateVector3 positionInterpolation = new InterpolateVector3() { LerpT = 0.15f, Enabled = true };

        public event FieldEvent<Quaternion> rotationChanged;
        public InterpolateQuaternion rotationInterpolation = new InterpolateQuaternion() { LerpT = 0.15f, Enabled = true };

        public Vector3 position
        {
            get { return _position; }
            set
            {
                if (_position == value)
                {
                    return;
                }
                _dirtyFields[0] |= 0x1;
                _position = value;
                hasDirtyFields = true;
            }
        }

        public Quaternion rotation
        {
            get { return _rotation; }
            set
            {
                if (_rotation == value)
                {
                    return;
                }
                _dirtyFields[0] |= 0x2;
                _rotation = value;
                hasDirtyFields = true;
            }
        }

        public override void Initialize(long createId, bool isOwner = false)
        {
            base.Initialize(createId, isOwner);
            this.OnAwake();     //创建对象
        }

        public virtual void OnAwake()
        {
            if (IsOwner)
            {
                networkMgr.AddNetworkObject(this);
            }
        }

        public virtual void OnStart()
        {
            NpcData = new NPCData();
            if (IsOwner)
            {
                npcMgr.Owner = this;

                rigidbody = gameObject.GetComponent<Rigidbody2D>();
                rigidbody.isKinematic = false;
                rigidbody.velocity = Vector2.zero;
            }
        }

        public virtual void OnUpdate()
        {
            if (NpcData == null)
            {
                return;
            }
            if (IsOwner)
            {
                position = transform.position;
                rotation = transform.rotation;
            }
            else
            {
                transform.position = position;
                transform.rotation = rotation;
            }
        }

        protected void OnRotate(float angle)
        {
            transform.DOLocalRotate(new Vector3(0, 0, angle), 1);
        }

        /// <summary>
        /// 初始化插值字段
        /// </summary>
        public void InitInterpolateFields(Vector3 position, Quaternion rotation)
        {
            _position = position;
            _rotation = rotation;

            transform.position = position;
            transform.rotation = rotation;

            positionInterpolation.current = position;
            positionInterpolation.target = position;

            rotationInterpolation.current = rotation;
            rotationInterpolation.target = rotation;
        }

        /// <summary>
        /// 插值字段更新
        /// </summary>
        public override void InterpolateUpdate()
        {
            if (IsOwner)
            {
                return;
            }
            if (positionInterpolation.Enabled && !positionInterpolation.current.Near(positionInterpolation.target, 0.0015f))
            {
                _position = (Vector3)positionInterpolation.Interpolate();
                this.RunChange_Position(positionInterpolation.Timestep);
            }
            if (rotationInterpolation.Enabled && !rotationInterpolation.current.Near(rotationInterpolation.target, 0.0015f))
            {
                _rotation = (Quaternion)rotationInterpolation.Interpolate();
                this.RunChange_Rotation(rotationInterpolation.Timestep);
            }
 	        base.InterpolateUpdate();
        }

        /// <summary>
        /// 更新插值字段
        /// </summary>
        public void ReadDirtyFields(NetDataReader reader)
        {
            ulong timestep = 0;
            reader.GetBytes(_readDirtyFlags, 1);
            if ((0x1 & _readDirtyFlags[0]) != 0)
            {
                if (positionInterpolation.Enabled)
                {
                    positionInterpolation.target = reader.GetVector3();
                    positionInterpolation.Timestep = timestep;
                }
                else
                {
                    _position = reader.GetVector3();
                    this.RunChange_Position(timestep);
                }
            }
            if ((0x2 & _readDirtyFlags[0]) != 0)
            {
                if (rotationInterpolation.Enabled)
                {
                    rotationInterpolation.target = reader.GetQuaternion();
                    rotationInterpolation.Timestep = timestep;
                }
                else
                {
                    _rotation = reader.GetQuaternion();
                    this.RunChange_Rotation(timestep);
                }
            }
        }

        private void RunChange_Position(ulong timestep)
        {
            if (positionChanged != null)
            {
                positionChanged(_position, timestep);
            }
        }

        private void RunChange_Rotation(ulong timestep)
        {
            if (rotationChanged != null)
            {
                rotationChanged(_rotation, timestep);
            }
        }

        protected override NetDataWriter SerializeDirtyFields()
        {
            writer.Reset();
            writer.Put(NetworkId);
            writer.Put(_dirtyFields);

            if ((0x1 & _dirtyFields[0]) != 0)
            {
                writer.Put(position);
            }
            if ((0x2 & _dirtyFields[0]) != 0)
            {
                writer.Put(rotation);
            }
            return writer;
        }

        public virtual void OnDispose()
        {
            if (IsOwner)
            {
                networkMgr.RemoveNetworkObject(this);
            }
            if (viewObject != null)
            {
                Destroy(viewObject);
            }
        }

        public virtual void OnDestroy()
        {
        }


        public virtual void OnTriggerEnter2D(Collider2D coll)
        {
        }
    }
}