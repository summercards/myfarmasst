using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Farming/Seed Plant Data", fileName = "SeedPlantDataSO")]
public class SeedPlantDataSO : ScriptableObject
{
    [Serializable]
    public class GrowthStage
    {
        [Tooltip("�ý׶εĳ���ʱ�䣨�룩")] public float duration = 5f;
        [Tooltip("�ý׶�ʹ�õ����Ԥ�ƣ��ɿգ�Ϊ��������ۣ�")]
        public GameObject visual;
    }

    [Serializable]
    public class Entry
    {
        [Header("Plant from Item")]
        [Tooltip("������ֲ����ƷID��ֱ����������Ʒ���� apple / banana��")]
        public string plantItemId;

        [Tooltip("���������Ԥ�ƣ�������� CropPlant�����ɽű��Զ��ӣ�")]
        public GameObject cropPrefab;

        [Header("Growth")]
        [Tooltip("�����µ�����Ľ׶Σ����ϵ������μ��")]
        public GrowthStage[] stages;

        [Header("Harvest (�ջ�)")]
        [Tooltip("�����ջ�ʱ����ҵ���ƷID��������Ĭ��=plantItemId")]
        public string produceId;
        [Tooltip("�����������䣨�����䣩")]
        public int produceMin = 1;
        public int produceMax = 3;

        [Header("Planting")]
        [Tooltip("��ֲ��������󴥣�")] public float plantCooldown = 0.2f;
        [Tooltip("����ʱ������ײ�̧�����ĸ߶�����")] public float spawnYOffset = 0.02f;

        // ����ʱ��ȡ��ʵ�����ڵ����ID����Ϊ������˵� plantItemId��
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
