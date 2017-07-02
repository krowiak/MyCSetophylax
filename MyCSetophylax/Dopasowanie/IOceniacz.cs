namespace MyCSetophylax.Dopasowanie
{
    public interface IOceniacz
    {
        double Ocen(Mrowka mrowka, (int x, int y) pozycjaMrowki);
    }
}