using UnityEngine;
using UnityEngine.UI;

public class StructureButton : MonoBehaviour
{
    public GameObject Structure;

    void Start()
    {
        Button button_component = GetComponent<Button>();
        Debug.Assert(button_component != null);
        button_component.onClick.AddListener(BuildStructure);
    }

    void BuildStructure()
    {
        GameManager.Instance.HandController.Build(Structure);

        // Close popupmenu
        transform.root.gameObject.SetActive(false);
    }
}
