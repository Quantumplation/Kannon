using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kannon.Components
{
    public interface IRenderableComponent
    {
        void Load();
        void Render();
    }
}
