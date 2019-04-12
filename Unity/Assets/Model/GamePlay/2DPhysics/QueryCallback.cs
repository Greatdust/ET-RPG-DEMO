using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public class PolyshapeQueryCallback : IQueryCallback
    {
        public List<Unit> units = new List<Unit>();

        public bool QueryCallback(Fixture fixture)
        {
            Unit unit = fixture.UserData as Unit;
            if (unit != null)
                units.Add(unit);
            return true;
        }
    }
}
