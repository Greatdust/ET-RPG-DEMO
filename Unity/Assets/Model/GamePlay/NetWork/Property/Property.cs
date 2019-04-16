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

    public class Property_CharacterState : IProperty<Dictionary<int, bool>>
    {
        public CharacterStateComponent characterState;

        public Dictionary<int, bool> Get()
        {
            return characterState.StateDic;
        }

        public Property_CharacterState(Unit unit)
        {
            characterState = unit.GetComponent<CharacterStateComponent>();
        }

        public IProperty GetCopy()
        {
            //角色的特殊状态不预测. 所以直接引用相同的
            return new Property_CharacterState(characterState.GetParent<Unit>());
        }

        public void Set(Dictionary<int, bool> t)
        {
            throw new NotImplementedException();
        }

        public void Set(SpecialStateType specialStateType, bool value)
        {
            characterState.Set(specialStateType, value);
        }

        public bool Get(SpecialStateType specialStateType)
        {
            return characterState.Get(specialStateType);
        }
    }



}
