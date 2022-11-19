using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject _wallObject;
    [SerializeField] private GameObject _wallPrefab = null;
    [SerializeField] private Material _previewMat = null;
    [SerializeField] private Material _invalidPreviewMat = null;
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
            edge.EdgeBuilding = Instantiate(_wallPrefab, edge.EdgePosition, Quaternion.Euler(0, 0, 0),_wallObject.transform);
            edge.EdgeBuilding.transform.forward = GetWallDirection(tileHit, edge);
            _preview.SetActive(false);
        }
    }

    private Vector3 GetWallDirection(Tile tile, Edge edge)
    {

        Vector3 tileDirection = edge.EdgePosition - tile.Position;

        return tileDirection;
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
        if (edge.EdgeBuilding != null)
        {
            InValidPreview();
        }
        else
        {
            ValidPreview();
        }
        _preview.transform.position = edge.EdgePosition;
        _preview.transform.forward = GetWallDirection(tileHit, edge);

    }

    private void CreatePreview()
    {
        _preview = Instantiate(_wallPrefab);
        ValidPreview();
    }

    private void ValidPreview()
    {
        _preview.GetComponentInChildren<Renderer>().sharedMaterial = _previewMat;

    }

    private void InValidPreview()
    {
        _preview.GetComponentInChildren<Renderer>().sharedMaterial = _invalidPreviewMat;

    }
}
