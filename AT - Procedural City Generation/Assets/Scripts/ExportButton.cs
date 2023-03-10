using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor.Formats.Fbx.Exporter;
using System.IO;
using System;

public class ExportButton : MonoBehaviour
{
    [SerializeField] GameObject _exportables;
    private Button _button;

    private void Awake()
    {
        _button = GetComponent<Button>();
        _button.interactable = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<BuildingManager>().GenerationComplete += EnableButton;
    }

    private void EnableButton()
    {
        _button.interactable = true;
    }

    public void StartExporting()
    {
        MeshRenderer[] renderables = _exportables.GetComponentsInChildren<MeshRenderer>();
        //UnityEngine.Object test = renderables[0].gameObject;

        UnityEngine.Object[] toExport = Array.ConvertAll(renderables, item => (UnityEngine.Object)item.gameObject);

        ExportGameObjects(toExport);
    }

    private static void ExportGameObjects(UnityEngine.Object[] objects)
    {
        string filePath = Path.Combine(Application.dataPath, "Export.fbx");
        Debug.Log("File exported to: " + filePath);
        ModelExporter.ExportObjects(filePath, objects);
    }

}
