using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// ���ױ�����壺
/// - �Զ����������ɸ���
/// - ��ʾͼ��/����
/// - �������л�������Ʒ
/// - I ����/�أ��ɽ��ã�
/// ������PlayerInventoryHolder��ActiveItemController��ItemDatabaseSO
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("Refs")]
    public PlayerInventoryHolder playerInv;      // ֱ���� Player ����������
    public ActiveItemController activeCtrl;     // ֱ���� Player ����������
    public ItemDatabaseSO itemDB;         // һ��� playerInv.itemDB ȡ��Ҳ���ֶ�ָ��

    [Header("UI")]
    public GameObject panelRoot;                 // ���������ڵ㣨���忪/�أ�
    public Transform gridRoot;                  // �Ÿ��ӵĸ����壨�� GridLayoutGroup��
    public InventorySlotUI slotPrefab;           // ��λԤ��
    public Sprite emptySprite;                   // �ո�ռλͼ���ɿգ�
    [Range(0, 1f)] public float emptyIconAlpha = 0.15f;

    [Header("Options")]
    public bool buildOnAwake = true;             // ����ʱ��������
    public bool toggleWithKey = true;            // �ð����л�
    public KeyCode toggleKey = KeyCode.I;

    readonly List<InventorySlotUI> _slots = new();

    void Reset()
    {
        if (!playerInv) playerInv = FindObjectOfType<PlayerInventoryHolder>();
        if (!activeCtrl) activeCtrl = FindObjectOfType<ActiveItemController>();
        if (!itemDB && playerInv) itemDB = playerInv.itemDB;
    }

    void Awake()
    {
        if (!itemDB && playerInv) itemDB = playerInv.itemDB;
        if (buildOnAwake) BuildSlots();
        RefreshAll();
    }

    void OnEnable()
    {
        if (playerInv != null) playerInv.OnInventoryChanged += RefreshAll;
        if (activeCtrl != null) activeCtrl.OnActiveChanged += _ => RefreshAll();
        RefreshAll();
    }

    void OnDisable()
    {
        if (playerInv != null) playerInv.OnInventoryChanged -= RefreshAll;
        if (activeCtrl != null) activeCtrl.OnActiveChanged -= _ => RefreshAll();
    }

    void Update()
    {
        if (toggleWithKey && Input.GetKeyDown(toggleKey))
        {
            TogglePanel();
        }
    }

    public void TogglePanel()
    {
        if (!panelRoot) return;
        panelRoot.SetActive(!panelRoot.activeSelf);
        if (panelRoot.activeSelf) RefreshAll();
    }

    /// <summary>���ݵ�ǰ��������������λUI</summary>
    public void BuildSlots()
    {
        _slots.Clear();
        if (!gridRoot || !slotPrefab) return;

        // ��վ�������
        for (int i = gridRoot.childCount - 1; i >= 0; i--)
            Destroy(gridRoot.GetChild(i).gameObject);

        int capacity = GetCapacity();
        for (int i = 0; i < capacity; i++)
        {
            var slot = Instantiate(slotPrefab, gridRoot);
            slot.Setup(this, i);
            _slots.Add(slot);
        }
    }

    /// <summary>ȫ��ˢ�£������仯ʱ���ؽ���</summary>
    public void RefreshAll()
    {
        if (!_isPanelVisible()) return;

        // �����仯���ؽ� UI
        if (_slots.Count != GetCapacity())
        {
            BuildSlots();
        }

        for (int i = 0; i < _slots.Count; i++)
        {
            _slots[i].Refresh();
        }
    }

    public ItemStack GetStack(int index)
    {
        if (playerInv == null || playerInv.Inventory == null || playerInv.Inventory.slots == null) return null;
        if (index < 0 || index >= playerInv.Inventory.slots.Length) return null;
        return playerInv.Inventory.slots[index];
    }

    public bool IsActiveId(string id)
    {
        return activeCtrl != null && !string.IsNullOrEmpty(id) && activeCtrl.ActiveId == id;
    }

    public void OnSlotLeftClick(int index)
    {
        var s = GetStack(index);
        if (s == null || string.IsNullOrEmpty(s.id) || s.count <= 0) return;
        if (activeCtrl != null) activeCtrl.SetActive(s.id, prefer: true);
        RefreshAll();
    }

    // �����Ҽ��������ɿ��Ŵ˷��������������еĶ����߼�
    // public void OnSlotRightClick(int index) { ... }

    public Sprite ResolveIcon(string id)
    {
        if (string.IsNullOrEmpty(id)) return emptySprite;
        var so = (itemDB != null) ? itemDB.Get(id) : null;   // ������ݿⷽ�������� Get�������
        if (so == null) return emptySprite;

        // �� ��� ItemSO ��ͼ���ֶβ��� "icon"�����������е� "icon" �ĳ�����ֶ���
        return so.icon != null ? so.icon : emptySprite;
    }

    int GetCapacity()
    {
        if (playerInv == null || playerInv.Inventory == null || playerInv.Inventory.slots == null) return 0;
        return playerInv.Inventory.slots.Length;
    }

    bool _isPanelVisible()
    {
        return panelRoot == null || panelRoot.activeInHierarchy;
    }
}
