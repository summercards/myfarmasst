using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Farming/Seed Plant Data", fileName = "SeedPlantDataSO")]
public class SeedPlantDataSO : ScriptableObject
{
    [Serializable]
    public class GrowthStage
    {
        [Tooltip("该阶段的持续时间（秒）")] public float duration = 5f;
        [Tooltip("该阶段使用的外观预制（可空；为空则不切外观）")]
        public GameObject visual;
    }

    [Serializable]
    public class Entry
    {
        [Header("Plant from Item")]
        [Tooltip("用于种植的物品ID（直接用现有物品：如 apple / banana）")]
        public string plantItemId;

        [Tooltip("作物根对象预制（上面需挂 CropPlant，或由脚本自动加）")]
        public GameObject cropPrefab;

        [Header("Growth")]
        [Tooltip("从种下到成熟的阶段（从上到下依次激活）")]
        public GrowthStage[] stages;

        [Header("Harvest (收获)")]
        [Tooltip("成熟收获时给玩家的物品ID；留空则默认=plantItemId")]
        public string produceId;
        [Tooltip("掉落数量区间（闭区间）")]
        public int produceMin = 1;
        public int produceMax = 3;

        [Header("Planting")]
        [Tooltip("种植间隔（防误触）")] public float plantCooldown = 0.2f;
        [Tooltip("种下时将作物底部抬离地面的高度修正")] public float spawnYOffset = 0.02f;

        // 运行时获取“实际用于掉落的ID”（为空则回退到 plantItemId）
        public string GetFinalProduceId() => string.IsNullOrEmpty(produceId) ? plantItemId : produceId;
    }

    public List<Entry> entries = new();

    public Entry GetByPlantItemId(string id)
    {
        if (string.IsNullOrEmpty(id)) return null;
        for (int i = 0; i < entries.Count; i++)
        {
            var e = entries[i];
            if (e != null && e.plantItemId == id) return e;
        }
        return null;
    }
}
