using Microsoft.Xna.Framework;
namespace Kannon
{
    public static class Entry
    {
        public static void Main()
        {
            using (XNAGame game = new XNAGame())
            {
                game.Run();
            }
        }
    }
}