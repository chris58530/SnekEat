using Core.MVC;
using UnityEngine;

public class PortalView : BaseView<PortalViewMediator>
{
    [SerializeField] private GameObject root;
    [SerializeField] private PortalObjectView portalObjectViewPrefab;
    [SerializeField] private GameObject holeMask;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPortal();
        }
    }
    private void Start()
    {
        ShowHoleMask(false);
        SpawnPortal();
    }

    public void ResetView()
    {
        foreach (Transform child in root.transform)
        {
            Destroy(child.gameObject);
        }
    }
    public void SpawnPortal()
    {
        Vector2 vector2 = new Vector2(Random.Range(GameMathService.generateAreaMin.x, GameMathService.generateAreaMax.x), Random.Range(GameMathService.generateAreaMin.y, GameMathService.generateAreaMax.y));
        PortalObjectView portalObjectView = Instantiate(portalObjectViewPrefab, vector2, Quaternion.identity);
        portalObjectView.transform.SetParent(root.transform);
    }

    public void ShowHoleMask(bool isShow, Transform snekTransform = null, Vector3? portalPosition = null)
    {
        holeMask.SetActive(isShow);
        if (isShow && snekTransform != null && portalPosition != null)
        {
            holeMask.transform.position = portalPosition.Value;
            holeMask.transform.rotation = snekTransform.rotation;
        }
    }
}
