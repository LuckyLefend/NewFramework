using System;
using System.Collections.Generic;
using MasterServer;

using Mathf = UnityEngine.Mathf;

namespace MasterServer.Manager
{
    public class FoodManager
    {
        private static FoodManager instance;

        public static FoodManager Instance
        {
            get
            {
                if (instance == null) 
                {
                    instance = new FoodManager();
                }
                return instance;
            }
        }
        private Dictionary<ulong, FoodInfo> foods = new Dictionary<ulong, FoodInfo>();

        public Dictionary<ulong, FoodInfo> Foods
        {
            get
            {
                return foods;
            }
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void Initialize()
        {
            foods.Clear();
            uint refCount = 0;
            var matrixs = ConfigManager.Instance.Matrixs;
            foreach (var de in matrixs)
            {
                var center = new Vector3(de.Value.pos_x, de.Value.pos_y, 0);
                var radius = 0f;
                var canCreate = false;
                var type = FoodType.None;
                switch (de.Value.type)
                {
                    case MatrixType.RedPoint:
                        radius = 220f;
                        canCreate = true;
                        type = FoodType.Red;
                    break;
                    case MatrixType.WathetPoint:
                        radius = 220f;
                        canCreate = true;
                        type = FoodType.Blue;
                    break;
                    case MatrixType.HairBall:
                        radius = 150;
                        canCreate = true;
                        type = FoodType.Green;
                    break;
                }
                if (canCreate)
                {
                    var angle = RandomArray(AppConst.PerChildCount, 0, 360);
                    for (int j = 0; j < AppConst.PerChildCount; j++)
                    {
                        var pos = RandomCircle(center, radius, angle[j]);
                        var food = new FoodInfo();
                        food.fid = ++refCount;
                        food.pos_x = pos.x;
                        food.pos_y = pos.y;
                        food.type = type;
                        foods.Add(food.fid, food);
                    }
                }
            }
        }

        Vector3 RandomCircle(Vector3 center, float radius, float angle)
        {
            Vector3 pos = new Vector3(0, 0, 0);
            pos.x = center.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad);
            pos.y = center.y + radius * Mathf.Cos(angle * Mathf.Deg2Rad);
            pos.z = angle;
            return pos;
        }

        public int[] RandomArray(int Number, int minNum, int maxNum)
        {
            int j;
            int[] b = new int[Number];
            Random r = new Random();
            for (j = 0; j < Number; j++)
            {
                int i = r.Next(minNum, maxNum + 1);
                int num = 0;
                for (int k = 0; k < j; k++)
                {
                    if (b[k] == i)
                    {
                        num = num + 1;
                    }
                }
                if (num == 0)
                {
                    b[j] = i;
                }
                else
                {
                    j = j - 1;
                }
            }
            return b;
        }

        public void RemoveFood(ulong foodid)
        {
            foods.Remove(foodid);
        }
    }
}
