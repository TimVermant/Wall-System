using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab = null;
    [SerializeField] private Material _previewMat = null;
    [SerializeField] private GridManager _gridManager = null;
    private GameObject _preview = null;




    private void Update()
    {
        RaycastHit hit = new();
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100))
        {
            DrawPreview(hit.point);
            if (Input.GetMouseButtonDown(0))
            {

                Build(hit.point);

            }
        }
    }

    private void Build(Vector3 position)
    {
        if (_wallPrefab)
        {

            Tile tileHit = new();
            Edge edge = new();
            _gridManager.GetBuildInfo(position, out tileHit, out edge);
            if (edge.EdgeBuilding != null)
            {
                return;
            }
            edge.EdgeBuilding = Instantiate(_wallPrefab, edge.EdgePosition, Quaternion.Euler(0, 0, 0));
            _preview.SetActive(false);
        }
    }

    private void DrawPreview(Vector3 position)
    {
        if (_preview == null)
        {
            CreatePreview();
        }
        _preview.SetActive(true);
        Tile tileHit = new();
        Edge edge = new();
        _gridManager.GetBuildInfo(position, out tileHit, out edge);
        _preview.transform.position = edge.EdgePosition;
    }

    private void CreatePreview()
    {
        _preview = Instantiate(_wallPrefab);
        _preview.GetComponentInChildren<Renderer>().sharedMaterial = _previewMat;

    }
}
