using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.UI;

public class UIRayVisibility : MonoBehaviour
{
    public XRRayInteractor rayInteractor;
    public float raycastMaxDistance = 100f;
    public LayerMask uiLayerMask;

    private LineRenderer lineRenderer;

    void Start()
    {
        if (rayInteractor == null)
            rayInteractor = GetComponent<XRRayInteractor>();

        lineRenderer = rayInteractor.GetComponent<LineRenderer>();
        
        // Initially hide the ray
        SetRayVisibility(false);
    }

    void Update()
    {
        bool isPointingAtUI = IsPointingAtUI();
        SetRayVisibility(isPointingAtUI);

        // Enable/disable the interactor based on UI targeting
        rayInteractor.enabled = isPointingAtUI;
    }

    bool IsPointingAtUI()
    {
        RaycastHit hit;
        if (Physics.Raycast(rayInteractor.rayOriginTransform.position, rayInteractor.rayOriginTransform.forward, out hit, raycastMaxDistance, uiLayerMask))
        {
            return hit.collider.gameObject.GetComponent<Graphic>() != null;
        }
        return false;
    }

    void SetRayVisibility(bool visible)
    {
        if (lineRenderer != null)
            lineRenderer.enabled = visible;
    }
}