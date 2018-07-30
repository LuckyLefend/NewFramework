using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

namespace FirClient.Manager
{
    public enum TimerType
    {
        Once,
        Loop,
    }

    public enum TimerState
    {
        Running,
        Delete,
        Stop,
    }

    public class TimerInfo
    {
        public string name;
        public float expires;
        public float tick;
        public float interval;
        public TimerState state;
        public object param;
        public Action<object> func;
    }

    public class TimerManager : BaseManager
    {
        private float interval = 0;
        private object mlock = new object();
        private Dictionary<string, TimerInfo> objects = new Dictionary<string, TimerInfo>();

        public override void Initialize()
        {
            isFixedUpdate = true;
        }

        public override void OnFixedUpdate(float deltaTime)
        {
            this.Run(deltaTime);
        }

        public override void OnUpdate(float deltaTime)
        {
        }

        /// <summary>
        /// 停止计时器
        /// </summary>
        public void StopTimer()
        {
            //CancelInvoke("Run");
        }

        /// <summary>
        /// 添加计时器事件
        /// </summary>
        /// <param name="name"></param>
        /// <param name="o"></param>
        public void AddTimer(string name, float expires, float interval, Action<object> func, object param = null)
        {
            lock (mlock)
            {
                if (!objects.ContainsKey(name))
                {
                    var timer = new TimerInfo();
                    timer.name = name;
                    timer.interval = interval;
                    timer.func = func;
                    timer.param = param;
                    timer.expires = expires;
                    timer.state = TimerState.Running;
                    objects.Add(name, timer);
                }
            }
        }

        /// <summary>
        /// 删除计时器事件
        /// </summary>
        /// <param name="name"></param>
        public void RemoveTimer(string name)
        {
            TimerInfo obj = null;
            if (objects.TryGetValue(name, out obj))
            {
                obj.state = TimerState.Delete;
            }
        }

        /// <summary>
        /// 停止计时器事件
        /// </summary>
        /// <param name="info"></param>
        public void StopTimer(string name)
        {
            TimerInfo obj = null;
            if (objects.TryGetValue(name, out obj))
            {
                obj.state = TimerState.Stop;
            }
        }

        /// <summary>
        /// 继续计时器事件
        /// </summary>
        /// <param name="info"></param>
        public void ResumeTimer(string name)
        {
            TimerInfo obj = null;
            if (objects.TryGetValue(name, out obj))
            {
                obj.state = TimerState.Running;
            }
        }

        /// <summary>
        /// 计时器运行
        /// </summary>
        void Run(float deltaTime)
        {
            if (objects.Count == 0)
            {
                return;
            }
            foreach (var de in objects)
            {
                var timer = de.Value;
                if (timer.state == TimerState.Running) 
                {
                    timer.tick += deltaTime;
                    if (timer.expires > 0)
                    {
                        if (timer.tick >= timer.expires)
                        {
                            timer.func.Invoke(timer.param);
                            timer.state = TimerState.Delete;
                        }
                    }
                    else
                    {
                        if (timer.tick >= timer.interval)
                        {
                            timer.tick = 0;
                            timer.func.Invoke(timer.param);
                        }
                    }
                }
            }
            /////////////////////////清除标记为删除的事件///////////////////////////
            lock (mlock)
            {
                var buffer = new List<TimerInfo>(objects.Values);
                foreach (var timer in buffer)
                {
                    if (timer.state == TimerState.Delete)
                    {
                        objects.Remove(timer.name);
                    }
                }
            }
        }

        public override void OnDispose()
        {
        }
    }
}