using Microsoft.Xna.Framework;
using System.Xml;

namespace Kannon
{
    public static class Entry
    {
        public static void Main()
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("Test.xml");
            EntityFactory.Parse(doc.LastChild);
            using (XNAGame game = new XNAGame())
            {
                game.Run();
            }
        }
    }
}