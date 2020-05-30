public class ActivateClone : AbstractClone
{
    public override void OnCloneAwake()
    {
        gameObject.SetActive(false);
    }

    public override void OnCloneEnable(Portal sender, Portal destination)
    {
        gameObject.SetActive(true);
    }

    public override void OnCloneDisable(Portal sender, Portal destination)
    {
        gameObject.SetActive(false);
    }
}