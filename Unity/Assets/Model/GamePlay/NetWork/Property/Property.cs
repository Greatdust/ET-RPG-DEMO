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
        private Vector3 position;

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
            Log.Debug("更新新位置记录 " + t.ToString());
            position = t;
        }


    }

}
