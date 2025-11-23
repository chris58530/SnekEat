using Core.MVC;
using UnityEngine;

public class PortalView : BaseView<PortalViewMediator>
{
    [SerializeField] private GameObject root;
    [SerializeField] private PortalObjectView portalObjectViewPrefab;

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            SpawnPortal();
        }
    }
    public void SpawnPortal()
    {
        Vector2 vector2 = new Vector2(Random.Range(GameMathService.generateAreaMin.x, GameMathService.generateAreaMax.x), Random.Range(GameMathService.generateAreaMin.y, GameMathService.generateAreaMax.y));
        PortalObjectView portalObjectView = Instantiate(portalObjectViewPrefab, vector2, Quaternion.identity);
        portalObjectView.transform.SetParent(root.transform);
    }
}
