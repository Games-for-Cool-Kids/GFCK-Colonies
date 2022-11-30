using UnityEngine;
using UnityEngine.UI;

public class StructureButton : MonoBehaviour
{
    public GameObject Structure;

    void Start()
    {
        Button button_component = GetComponent<Button>();
        button_component.onClick.AddListener(BuildStructure);
    }

    void OnEnable()
    {
        Button button_component = GetComponent<Button>();
        button_component.onClick.RemoveAllListeners();
        button_component.onClick.AddListener(BuildStructure);
    }


    void BuildStructure()
    {
        GameManager.Instance.HandController.Build(Structure);

        // Close popup-menu
        transform.root.gameObject.SetActive(false);
    }
}
