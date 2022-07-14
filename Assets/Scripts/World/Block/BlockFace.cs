public class BlockFace
{
    public enum DIRECTION
    {
        TOP,
        SIDE,
    }

    public DIRECTION direction;

    public int x, y; // Index for blockface texture lookup
}
