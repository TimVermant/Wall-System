using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [SerializeField] private GameObject _wallPrefab = null;
    [SerializeField] private Material _previewMat = null;

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
                //Debug.Log(hit.transform.name);
                //Debug.Log("hit");
                Build(hit.point);
            }
        }
    }

    private void Build(Vector3 position)
    {
        if (_wallPrefab)
        {
            Instantiate(_wallPrefab, position, Quaternion.Euler(-90, 0, 0));
            _preview.SetActive(false);
        }
    }

    private void DrawPreview(Vector3 location)
    {
        if( _preview == null)
        {
            CreatePreview();
        }
        _preview.SetActive(true);
        _preview.transform.position = location;
    }

    private void CreatePreview()
    {
        _preview = Instantiate(_wallPrefab);
        _preview.GetComponentInChildren<Renderer>().sharedMaterial = _previewMat;

    }
}
