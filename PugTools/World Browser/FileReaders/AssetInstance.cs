using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SlimDX;

namespace FileFormats
{
    public class AssetInstance
    {
        public long Id;
        public long AssetId;
        public long ParentInstanceId;
        public RoomSpecification Room;        
        public Vector3 Position;        
        public Vector3 Scale;        
        public Vector3 Rotation;
        public Matrix Transform;
        public bool Hidden;

        public AssetInstance()
        {
            this.Id = 0;
            this.AssetId = 0;
            this.ParentInstanceId = 0;
            this.Room = null;
            this.Position = new Vector3();
            this.Rotation = new Vector3();
            this.Scale = new Vector3(1.0f, 1.0f, 1.0f);
            this.Hidden = false;
        }

        public void CalculateTransform()
        {
            this.Transform = (((Matrix.Scaling(this.Scale) * Matrix.RotationZ((float) ((this.Rotation.Z * Math.PI) / 180.0))) * Matrix.RotationX((float) ((this.Rotation.X * Math.PI) / 180.0))) * Matrix.RotationY((float) ((this.Rotation.Y * Math.PI) / 180.0))) * Matrix.Translation(this.Position);
            //this.Transform = Matrix.Translation(this.Position);
        }

        public Matrix GetAbsoluteTransform()
        { 
            long pId = this.ParentInstanceId;
            AssetInstance p;
            Matrix outputTransform = Matrix.Identity;
            outputTransform *= this.Transform;
            while (pId != 0)
            {
                if (!this.Room.InstancesById.ContainsKey(pId)) { break; }
                p = this.Room.InstancesById[pId];
                outputTransform *= p.Transform;
                pId = p.ParentInstanceId;
                /*
                this.Position.X += p.Position.X;
                this.Position.Y += p.Position.Y;
                this.Position.Z += p.Position.Z;
                */
            }

            return outputTransform;
        }


        public Vector3 GetAbsolutePosition()
        { 
            long pId = this.ParentInstanceId;
            AssetInstance p;
            Vector3 outputVec = new Vector3();
            while (pId != 0)
            {
                if (!this.Room.InstancesById.ContainsKey(pId)) { break; }
                p = this.Room.InstancesById[pId];
                outputVec.X += p.Position.X;
                outputVec.Y += p.Position.Y;
                outputVec.Z += p.Position.Z;
                pId = p.ParentInstanceId;
            }

            return outputVec;
        }

    }
}
