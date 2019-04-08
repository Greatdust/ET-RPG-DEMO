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
        public Vector3 position;

        public Vector3 Get()
        {
            return position;
        }
        

        public IProperty GetCopy()
        {
            return new Property_Position()
            {
                position = this.position
            };

        }

        public void Set(Vector3 t)
        {
            position = t;
        }


    }

    public class Property_Rotation : IProperty<Quaternion>
    {
        public Quaternion value;

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
