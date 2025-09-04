using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Refs")]
    public Image frame;              // 可空，用作背景/边框
    public Image icon;               // 物品图标
    public Text countText;          // 数量文本
    public GameObject highlight;     // 选中高亮（可空）

    [Header("Runtime")]
    public int slotIndex = -1;

    InventoryUI _root;

    public void Setup(InventoryUI root, int index)
    {
        _root = root;
        slotIndex = index;
        Refresh();
    }

    public void Refresh()
    {
        if (_root == null || slotIndex < 0) return;

        var stack = _root.GetStack(slotIndex);
        if (stack == null || string.IsNullOrEmpty(stack.id) || stack.count <= 0)
        {
            if (icon) { icon.sprite = _root.emptySprite; icon.color = new Color(1, 1, 1, _root.emptyIconAlpha); }
            if (countText) countText.text = "";
            if (highlight) highlight.SetActive(false);
            return;
        }

        // 图标
        var sp = _root.ResolveIcon(stack.id);
        if (icon)
        {
            icon.sprite = sp != null ? sp : _root.emptySprite;
            icon.color = new Color(1, 1, 1, 1f);
        }

        // 数量
        if (countText) countText.text = stack.count > 1 ? stack.count.ToString() : "";

        // 高亮：与当前激活物品一致时点亮
        if (highlight) highlight.SetActive(_root.IsActiveId(stack.id));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_root == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _root.OnSlotLeftClick(slotIndex);
        }
        // 如需右键丢弃，可在这里扩展：
        // else if (eventData.button == PointerEventData.InputButton.Right) { _root.OnSlotRightClick(slotIndex); }
    }
}
