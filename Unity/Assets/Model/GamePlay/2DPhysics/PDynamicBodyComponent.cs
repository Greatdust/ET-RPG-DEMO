using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class PDynamicBodyComponentAwakeComponent : AwakeSystem<PDynamicBodyComponent, Shape>
    {
        public override void Awake(PDynamicBodyComponent self,Shape shape)
        {

            self.Awake(shape);
        }
    }


    public class PDynamicBodyComponent : Component
    {
        public Body body;
        public Fixture fixture;

        public void Awake(Shape shape)
        {
            World world = PhysicWorldComponent.Instance.world;
            var bd = new BodyDef
            {
                Position = GetParent<Unit>().Position.ToVector2(),
                BodyType = BodyType.DynamicBody,
                FixedRotation = true,
                AllowSleep = false
            };

            body = world.CreateBody(bd);
            var fd = new FixtureDef
            {
                Shape = shape,
                Density = 20.0f,
                UserData = GetParent<Unit>(),
                Friction = 0,
                Filter = new Filter()
                {
                    CategoryBits = (ushort)GetParent<Unit>().UnitLayer,
                    MaskBits = (ushort)GetParent<Unit>().LayerMask
                }
            };

            fixture = body.CreateFixture(fd);

            GetParent<Unit>().OnPositionUpdate += UpdatePos;
            GetParent<Unit>().OnRotationUpdate += UpdateRot;
        }

        public void UpdatePos(UnityEngine.Vector3 vector3)
        {
            body.SetTransform(vector3.ToVector2(), body.GetAngle());
        }

        public void UpdateRot(UnityEngine.Quaternion quaternion)
        {
            body.SetTransform(body.GetPosition(), quaternion.ToRotation2D().Angle);
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            World world = PhysicWorldComponent.Instance.world;
            world.DestroyBody(this.body);
        }
    }
}
