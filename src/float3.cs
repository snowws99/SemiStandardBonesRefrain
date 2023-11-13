using PEPlugin.Pmd;
using PEPlugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PEPlugin.SDX;

namespace SemiStandardBonesRefrain
{
    public struct float3
    {
        public float X;

        public float Y;

        public float Z;

        public float x
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }

        public float y
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        public float z
        {
            get
            {
                return Z;
            }
            set
            {
                Z = value;
            }
        }

        public float r
        {
            get
            {
                return X;
            }
            set
            {
                X = value;
            }
        }

        public float g
        {
            get
            {
                return Y;
            }
            set
            {
                Y = value;
            }
        }

        public float b
        {
            get
            {
                return Z;
            }
            set
            {
                Z = value;
            }
        }

        public float3(float X, float Y, float Z)
        {
            this.X = X;
            this.Y = Y;
            this.Z = Z;
        }

        public float3(IPEVector3 vec)
        {
            X = vec.X;
            Y = vec.Y;
            Z = vec.Z;
        }

        public static float3 operator +(float3 a, float3 b)
        {
            return new float3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
        }

        public static float3 operator -(float3 a, float3 b)
        {
            return new float3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
        }

        public static float3 operator *(float3 a, float3 b)
        {
            return new float3(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
        }

        public static float3 operator *(float3 a, float b)
        {
            return new float3(a.X * b, a.Y * b, a.Z * b);
        }

        public static float3 operator *(float a, float3 b)
        {
            return new float3(a * b.X, a * b.Y, a * b.Z);
        }

        public static float3 operator /(float3 a, float3 b)
        {
            return new float3(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
        }

        public static float3 operator /(float3 a, float b)
        {
            return new float3(a.X / b, a.Y / b, a.Z / b);
        }

        public static float3 operator /(float a, float3 b)
        {
            return new float3(a / b.X, a / b.Y, a / b.Z);
        }

        public IPEVector3 ToIPEVec()
        {
            return PEStaticBuilder.Builder.CreateVector3(X, Y, Z);
        }

        public static float length(float3 vec)
        {
            return (float)Math.Sqrt(vec.x * vec.x + vec.y * vec.y + vec.z * vec.z);
        }

        public static float3 normalize(float3 vec)
        {
            float num = length(vec);
            if (num == 0f)
            {
                return vec;
            }
            return vec / num;
        }

        public static float dot(float3 a, float3 b)
        {
            return a.x * b.x + a.y * b.y + a.z * b.z;
        }

        public static float3 cross(float3 a, float3 b)
        {
            float x = a.y * b.z - a.z * b.y;
            float y = a.z * b.x - a.x * b.z;
            float z = a.x * b.y - a.y * b.x;
            return new float3(x, y, z);
        }

        public static float lerp(float x, float y, float s)
        {
            return x + s * (y - x);
        }

        public static float3 lerp(float3 x, float3 y, float s)
        {
            return x + s * (y - x);
        }


        public V3 ToV3()
        {
            return new V3(X, Y, Z);
        }
    }

}
