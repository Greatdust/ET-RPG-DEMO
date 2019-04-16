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
    public class PDynamicBodyComponentAwakeComponent : AwakeSystem<PDynamicBodyComponent, PBaseShape>
    {
        public override void Awake(PDynamicBodyComponent self, PBaseShape shape)
        {

            self.Awake(shape);
        }
    }


    public class PDynamicBodyComponent : Component
    {
        public Body body;
        public Fixture fixture;

        public PBaseShape baseShape;

        public void Awake(PBaseShape pShape)
        {
            baseShape = pShape;
            Shape shape = null;
            Unit unit = GetParent<Unit>();
            unit.UnitData = pShape.unitData;
            unit.OffsetY = pShape.offset.y;
            Log.Debug(pShape.offset.ToString());
            Log.Debug("Unit的Offset Y是{0}", unit.OffsetY);
            switch (pShape)
            {
                case PBoxShape boxShape:
                    //根据配置的信息,先设置Unit的一堆属性
    
                    unit.HalfHeight = boxShape.size.y / 2;

                    
                    //接着设置形状的数据
                    var box = new PolygonShape();
                    box.SetAsBox(boxShape.size.x, boxShape.size.z, pShape.offset.ToVector2(), BOX2DMathfUtils.ToAngle(boxShape.eulerAnglesY));
                    shape = box;
                    break;
                case PCircleShape pCircleShape:
                    unit.HalfHeight = pCircleShape.halfHeight;

                    var circle = new CircleShape();
                    circle.Radius = pCircleShape.radius;
                    circle.Position = pShape.offset.ToVector2();
                    shape = circle;
                    break;
            }

            World world = PhysicWorldComponent.Instance.world;

            var bd = new BodyDef
            {
                Position = GetParent<Unit>().Position.ToVector2(),
                BodyType = pShape.bodyType,
                FixedRotation = true,
                
                AllowSleep = false,
                
            };

            body = world.CreateBody(bd);
            Log.Debug(GetParent<Unit>().GameObject.name + " 在2D物理世界的位置是" + body.GetPosition());

            var fd = new FixtureDef
            {
                Shape = shape,
                Density = 20.0f,
                UserData = GetParent<Unit>(),
                Friction = 0,
                IsSensor = pShape.isSensor,
                Filter = new Filter()
                {
                    CategoryBits = (ushort)GetParent<Unit>().UnitData.unitLayer,
                    MaskBits = (ushort)GetParent<Unit>().UnitData.layerMask,
                    GroupIndex = (short)GetParent<Unit>().UnitData.groupIndex,
                    
                }
            };
      

            fixture = body.CreateFixture(fd);
            Log.Debug(fixture.GetAABB(0).LowerBound.ToString() + fixture.GetAABB(0).UpperBound.ToString());

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
