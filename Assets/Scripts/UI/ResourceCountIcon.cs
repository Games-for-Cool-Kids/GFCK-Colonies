using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceCountIcon : MonoBehaviour
{
    public void SetIcon(Sprite sprite)
    {
        gameObject.GetComponentInChildren<Image>().sprite = sprite;
    }

    public void setCount(int amount)
    {
        gameObject.GetComponentInChildren<TextMeshProUGUI>().text = amount.ToString();
    }

    public void SetVisible(bool visible)
    {
        gameObject.SetActive(visible);
    }
}
