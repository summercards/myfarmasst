using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI Refs")]
    public Image frame;              // �ɿգ���������/�߿�
    public Image icon;               // ��Ʒͼ��
    public Text countText;          // �����ı�
    public GameObject highlight;     // ѡ�и������ɿգ�

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

        // ͼ��
        var sp = _root.ResolveIcon(stack.id);
        if (icon)
        {
            icon.sprite = sp != null ? sp : _root.emptySprite;
            icon.color = new Color(1, 1, 1, 1f);
        }

        // ����
        if (countText) countText.text = stack.count > 1 ? stack.count.ToString() : "";

        // �������뵱ǰ������Ʒһ��ʱ����
        if (highlight) highlight.SetActive(_root.IsActiveId(stack.id));
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (_root == null) return;

        if (eventData.button == PointerEventData.InputButton.Left)
        {
            _root.OnSlotLeftClick(slotIndex);
        }
        // �����Ҽ�����������������չ��
        // else if (eventData.button == PointerEventData.InputButton.Right) { _root.OnSlotRightClick(slotIndex); }
    }
}
