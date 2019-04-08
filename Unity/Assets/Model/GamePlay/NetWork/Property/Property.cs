using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public interface IProperty
    {
        IProperty GetCopy();
    }

    public interface IProperty<T> : IProperty
    {
        T Get();
        void Set(T t);

    }

    public class Property_Position :IProperty<Vector3>
    {
        public Unit unit;

        public Property_Position(Unit unit)
        {
            this.unit = unit;
        }

        public Vector3 Get()
        {
            return unit.Position;
        }
        

        public IProperty GetCopy()
        {
            return new Property_Position(unit);

        }

        public void Set(Vector3 t)
        {
            unit.Position = t;
        }


    }

    public class Property_Rotation : IProperty<Quaternion>
    {
        private Quaternion value;

        public Quaternion Get()
        {
            return value;
        }


        public IProperty GetCopy()
        {
            return new Property_Rotation()
            {
                value = this.value
            };

        }

        public void Set(Quaternion t)
        {
            value = t;
        }


    }

}
