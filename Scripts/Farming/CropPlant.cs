using UnityEngine;

[DisallowMultipleComponent]
public class CropPlant : MonoBehaviour
{
    [Header("Runtime (ReadOnly)")]
    public bool isMature = false;

    SeedPlantDataSO.Entry _cfg;
    float _timer = 0f;
    int _stageIndex = 0;
    GameObject[] _stageVisuals;

    [Header("Interact")]
    [Tooltip("用于交互检测的触发器（可自动添加）")]
    public SphereCollider interactTrigger;
    [Tooltip("交互键，默认 E")]
    public KeyCode interactKey = KeyCode.E;

    public void Init(SeedPlantDataSO.Entry cfg)
    {
        _cfg = cfg;
        SetupStageVisuals();
        SetupTrigger();
        ApplyStage(0);
    }

    void Update()
    {
        if (_cfg == null || isMature) return;
        if (_cfg.stages == null || _cfg.stages.Length == 0) { isMature = true; return; }

        _timer += Time.deltaTime;
        var cur = _cfg.stages[_stageIndex];
        if (_timer >= cur.duration)
        {
            _timer = 0f;
            _stageIndex++;
            if (_stageIndex >= _cfg.stages.Length)
            {
                _stageIndex = _cfg.stages.Length - 1;
                isMature = true;
            }
            ApplyStage(_stageIndex);
        }
    }

    void ApplyStage(int index)
    {
        if (_stageVisuals == null) return;
        for (int i = 0; i < _stageVisuals.Length; i++)
            if (_stageVisuals[i]) _stageVisuals[i].SetActive(i == index);
    }

    void SetupStageVisuals()
    {
        if (_cfg == null) return;
        int n = (_cfg.stages != null) ? _cfg.stages.Length : 0;
        _stageVisuals = new GameObject[n];
        for (int i = 0; i < n; i++)
        {
            var v = _cfg.stages[i].visual;
            if (v != null)
            {
                var inst = Instantiate(v, transform);
                inst.transform.localPosition = Vector3.zero;
                inst.transform.localRotation = Quaternion.identity;
                _stageVisuals[i] = inst;
            }
        }
    }

    void SetupTrigger()
    {
        if (!interactTrigger)
        {
            interactTrigger = gameObject.AddComponent<SphereCollider>();
            interactTrigger.isTrigger = true;
            interactTrigger.radius = 0.6f;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (!isMature) return;
        if (!other.TryGetComponent<PlayerInventoryHolder>(out var holder)) return;

        if (Input.GetKeyDown(interactKey))
            HarvestTo(holder);
    }

    public void HarvestTo(PlayerInventoryHolder holder)
    {
        if (!isMature || holder == null || _cfg == null) return;

        string dropId = _cfg.GetFinalProduceId();  // 默认为 plantItemId
        int amount = Mathf.Clamp(Random.Range(_cfg.produceMin, _cfg.produceMax + 1), 0, 999);

        if (!string.IsNullOrEmpty(dropId) && amount > 0)
            holder.AddItem(dropId, amount);  // 直接进背包

        Destroy(gameObject);
    }
}
