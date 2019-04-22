using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace ETModel
{
    [Serializable]
    public struct Vector3Serializer
    {
        public float x;
        public float y;
        public float z;

        public void Fill(Vector3 vector3)
        {
            x = vector3.x;
            y = vector3.y;
            z = vector3.z;
        }

        public Vector3 ToV3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public struct Vector2Serializer
    {
        public float x;
        public float y;

        public void Fill(Vector2 vector2)
        {
            x = vector2.x;
            y = vector2.y;
        }


        public Vector2 ToV2()
        {
            return new Vector2(x, y);
        }
    }
    sealed class PreMergeToMergedDeserializationBinder : SerializationBinder
    {
        public override Type BindToType(string assemblyName, string typeName)
        {
            Type typeToDeserialize = null;

            // For each assemblyName/typeName that you want to deserialize to
            // a different type, set typeToDeserialize to the desired type.
            String exeAssembly = Assembly.GetExecutingAssembly().FullName;


            // The following line of code returns the type.
            typeToDeserialize = Type.GetType(String.Format("{0}, {1}",
                typeName, exeAssembly));
            Log.Debug(String.Format("原 {0} ------ {1}", typeName, assemblyName));
            Log.Debug(String.Format("现在 {0} -----  {1}", typeName, exeAssembly));
            return typeToDeserialize;
        }
    }
}
