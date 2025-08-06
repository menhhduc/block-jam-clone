public class Node
{
    public bool walkable = true;
    public int x, y;
    public Slot slot;

    public int gCost;
    public int hCost;

    public int fCost => gCost + hCost;
    public Node parent;

    public Node(int x, int y, Slot slot)
    {
        if (slot == null)
            walkable = false;
        else
        {
            this.slot = slot;
            if (slot.Mob != null)
                walkable = false;
        }
        this.x = x;
        this.y = y;
    }

    public void UpdateWalkableState()
    {
        if (slot == null)
        {
            walkable = false;
        }
        else
        {
            walkable = slot.Mob == null;
        }
    }
}