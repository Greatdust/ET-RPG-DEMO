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

        public Vector3 Get()
        {
            return unit.Position;
        }

        public Property_Position(Unit unit)
        {
            this.unit = unit;
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

    public class Property_InBattleState : IProperty<bool>
    {
        public bool value;

        public bool Get()
        {
            return value;
        }


        public IProperty GetCopy()
        {
            return new Property_InBattleState()
            {
                value = this.value
            };

        }

        public void Set(bool t)
        {
            value = t;
        }

    }

    public class Property_Die : IProperty<bool>
    {
        public bool value;

        public bool Get()
        {
            return value;
        }


        public IProperty GetCopy()
        {
            return new Property_InBattleState()
            {
                value = this.value
            };

        }

        public void Set(bool t)
        {
            value = t;
        }

    }

    public class Property_UnStoppable : IProperty<bool>
    {
        public bool value;

        public bool Get()
        {
            return value;
        }


        public IProperty GetCopy()
        {
            return new Property_InBattleState()
            {
                value = this.value
            };

        }

        public void Set(bool t)
        {
            value = t;
        }

    }


    public class Property_NotInControl : IProperty<bool>
    {
        public bool value;

        public bool Get()
        {
            return value;
        }


        public IProperty GetCopy()
        {
            return new Property_InBattleState()
            {
                value = this.value
            };

        }

        public void Set(bool t)
        {
            value = t;
        }

    }

    public class Property_Invicible : IProperty<bool>
    {
        public bool value;

        public bool Get()
        {
            return value;
        }


        public IProperty GetCopy()
        {
            return new Property_InBattleState()
            {
                value = this.value
            };

        }

        public void Set(bool t)
        {
            value = t;
        }

    }



}
