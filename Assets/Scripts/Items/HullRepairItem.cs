using UnityEngine;

public class HullRepairItem : SingleUseItem
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();

        OnUse.AddListener(RepairHull);
    }

    private void RepairHull(UseEvent arg0)
    {
        _logger.Trace("Repairing hull");
        var gameManager = GameManager.Instance;
        gameManager.GainHealth(1);
    }

    protected override bool CanBeUsedOnObject(GameObject obj) => true;
    protected override bool CanBeUsedOnGround() => true;
}
