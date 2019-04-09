using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETModel
{
	public class UnitPathComponent : Component
	{
        public Vector3 Target;

        private ABPathWrap abPath;

        public List<Vector3> Path = new List<Vector3>();

		public Vector3 ServerPos;

		public CancellationTokenSource CancellationTokenSource;

        public ABPathWrap ABPath
        {
            get
            {
                return this.abPath;
            }
            set
            {
                this.abPath?.Dispose();
                this.abPath = value;
            }
        }

	}
}