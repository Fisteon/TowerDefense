using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerMask : MonoBehaviour
{
    private Vector3 lastMousePosition;
    private Vector3 lastPosition;
    private Material baseMaterial;
    public Camera camera;
    public GameObject buildZone;
    public GameObject MaskModel;
    public bool outOfBounds;

    // Use this for initialization
    void Start()
    {
        baseMaterial = MaskModel.GetComponent<Renderer>().material;
        Move();
        lastMousePosition = Input.mousePosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (lastMousePosition != Input.mousePosition)
        {
            Move();
            lastMousePosition = Input.mousePosition;
        }

        outOfBounds = OutOfBounds();
    }

    private void Move()
    {
        Ray ray = camera.ScreenPointToRay(Input.mousePosition);
        float point;
        if (GameMaster.Master.ground.Raycast(ray, out point))
        {
            Vector3 clickLocation = GameMaster.Master.RoundTheLocation(ray.GetPoint(point));
            this.transform.position = clickLocation;
        }
    }

    private bool OutOfBounds()
    {
        RaycastHit rayInfo;
        if (Physics.Raycast(transform.position, Vector3.down, out rayInfo, 2f, LayerMask.NameToLayer("Tower Placement")))
        {
            if (rayInfo.collider.gameObject != buildZone)
            {
                ColorUnavailable();
                return true;
            }
        }
        else
        {
            ColorUnavailable();
            return true;
        }
        ColorAvailable();
        return false;
    }

    public void ColorUnavailable()
    {
        MaskModel.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 0f, 0f, 1f));
    }

    public void ColorAvailable()
    {
        MaskModel.GetComponent<Renderer>().material.SetColor("_Color", new Color(1, 1, 1, 1f));
    }
}
