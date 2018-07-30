using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MasterServer.View;
using Utility;

namespace MasterServer.Manager
{
    public class NPCManager : BaseBehaviour, IManager
    {
        public Dictionary<long, NPCView> mNpcs = new Dictionary<long, NPCView>();

        private int currIdentity = 0;
        private readonly object idlock = new object();
        private readonly object npcLock = new object();

        public NPCManager()
        {
            NpcMgr = this;
        }

        public void Initialize()
        {
            StartMonsterCreateDone();
        }

        void StartMonsterCreateDone()
        {
            TimerMgr.AddTimer("Monster Create", 2, delegate()
            {
                if (mNpcs.Count == 0)
                {
                    var spawnPoint = ConfigMgr.MonsterSpawnPoints;
                    var list = spawnPoint.ToList();
                    var index = AppUtil.Random(0, list.Count);

                    var npcid = DateTime.UtcNow.Ticks;
                    var view = new NPCView(npcid, NpcType.Monster);
                    view.position = list[index].Value;
                    mNpcs.Add(npcid, view);

                    Log.Info("Check Monster OnScene Count:>" + mNpcs.Count);
                }
            });
        }

        /// <summary>
        /// 新的标识
        /// </summary>
        /// <returns></returns>
        public int NewIdentity
        {
            get
            {
                lock (idlock)
                {
                    return ++currIdentity;
                }
            }
        }

        public void AddNpc(long npcId, NPCView view)
        {
            lock (npcLock)
            {
                mNpcs.Add(npcId, view);
            }
        }

        public NPCView GetNpc(long npcId)
        {
            lock (npcLock)
            {
                if (mNpcs.ContainsKey(npcId))
                {
                    return mNpcs[npcId];
                }
                return null;
            }
        }

        public bool RemoveNpc(long npcId)
        {
            lock (npcLock)
            {
                return mNpcs.Remove(npcId);
            }
        }


        public void OnFrameUpdate()
        {
        }

        public void OnDispose()
        {
            throw new NotImplementedException();
        }
    }
}
