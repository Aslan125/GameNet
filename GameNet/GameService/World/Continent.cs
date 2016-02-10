using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameNet.GameService.World
{
    public class Continent : IWorld
    {
        IWorld[] Locations;
        public Vector2 this[int index]
        {
            get
            {
                return Position[index];
            }

            set
            {
                Position[index] = value;
            }
        }

        public Vector2[] Position
        {
            get;

            set;
        }
    }
}
