
using System;
using System.Collections.Generic;
using System.Timers;
using LiteNetLib;
using UnityEngine;


    /// <summary>
    /// 
    /// </summary>
public class LagMonitor : GameBehaviour
{
    //private int interval;
    private int _lastReqTime;
    private Timer _pollTimer;
    private int _queueSize;
    private NetManager _net;
    private List<int> _valueQueue;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="sfs"></param>
    public LagMonitor(NetManager net)
        : this(net, 4, 10)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mpnet"></param>
    /// <param name="interval"></param>
    public LagMonitor(NetManager net, int interval)
        : this(net, interval, 10)
    {
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mpnet"></param>
    /// <param name="interval"></param>
    /// <param name="queueSize"></param>
    public LagMonitor(NetManager net, int interval, int queueSize)
    {
        if (interval < 1)
            interval = 1;
        _net = net;
        _queueSize = queueSize;
        _valueQueue = new List<int>();
        _pollTimer = new Timer();
        _pollTimer.Enabled = false;
        _pollTimer.AutoReset = true;
        _pollTimer.Elapsed += new ElapsedEventHandler(OnPollEvent);
        _pollTimer.Interval = interval * 1000;
    }

    /// <summary>
    /// 
    /// </summary>
    public void Destroy()
    {
        Stop();
        _pollTimer.Dispose();
        _net = null;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public int OnPingPong()
    {
        int item = DateTime.Now.Millisecond - _lastReqTime;
        if (_valueQueue.Count >= _queueSize)
        {
            _valueQueue.RemoveAt(0);
        }
        _valueQueue.Add(item);
        return AveragePingTime;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="source"></param>
    /// <param name="e"></param>
    private void OnPollEvent(object source, ElapsedEventArgs e)
    {
        Debugger.Log("********** Polling!!");
        _lastReqTime = DateTime.Now.Millisecond;
        //_net.Send(new PingPongRequest());
    }

    /// <summary>
    /// 
    /// </summary>
    public void Start()
    {
        if (!IsRunning)
        {
            _pollTimer.Start();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public void Stop()
    {
        if (IsRunning)
        {
            _pollTimer.Stop();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public int AveragePingTime
    {
        get
        {
            if (_valueQueue.Count == 0)
            {
                return 0;
            }
            int num = 0;
            foreach (int num2 in _valueQueue)
            {
                num += num2;
            }
            return (num / _valueQueue.Count);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public bool IsRunning
    {
        get
        {
            return _pollTimer.Enabled;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public int LastPingTime
    {
        get
        {
            if (_valueQueue.Count > 0)
            {
                return _valueQueue[_valueQueue.Count - 1];
            }
            return 0;
        }
    }
}

