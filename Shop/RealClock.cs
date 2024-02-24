namespace Shop
{
    public class RealClock: IClock
    {
        public DateTime Current() => DateTime.Now;

    }
}
