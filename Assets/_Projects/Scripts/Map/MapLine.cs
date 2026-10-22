using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class MapLine : MonoBehaviour
{
    [SerializeField] private Image _image;

    public int FromNodeId { get; private set; }
    public int ToNodeId { get; private set; }

    private void Awake()
    {
        if (_image == null)
            _image = GetComponent<Image>();

        _image.raycastTarget = false;
    }
    
    public void SetData(int fromId, int toId, Vector2 from, Vector2 to, float thickness)
    {
        FromNodeId = fromId;
        ToNodeId = toId;

        RectTransform rect = (RectTransform)transform;

        Vector2 direction = to - from;

        rect.anchorMin = new Vector2(0.5f, 0f);
        rect.anchorMax = new Vector2(0.5f, 0f);
        rect.pivot = new Vector2(0.5f, 0.5f);

        rect.anchoredPosition = (from + to) * 0.5f;

        rect.sizeDelta = new Vector2(direction.magnitude, thickness);

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        rect.localRotation = Quaternion.Euler(0f, 0f, angle);

        _image.raycastTarget = false;
    }

    public void SetColor(Color color)
    {
        if(_image != null)
            _image.color = color;
    }
}