public class PlayerStateBase
{
    protected Player owner;
    public PlayerStateBase(Player player)
    {
        owner = player;
    }
    public virtual void OnEnter()
    {

    }
    public virtual void OnUpdate()
    {

    }
    public virtual void OnExit()
    {

    }
    public virtual void OnFixedUpdate()
    {

    }
}