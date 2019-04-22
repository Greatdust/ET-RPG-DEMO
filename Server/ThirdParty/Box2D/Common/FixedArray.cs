using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Box2DSharp.Common
{
    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray2<T>
        where T : unmanaged
    {
        public T Value0;

        public T Value1;

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T* Values
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (T* ptr = &Value0)
                {
                    return ptr;
                }
            }
        }

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T this[int index]
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Values[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray3<T>
        where T : unmanaged
    {
        public T Value0;

        public T Value1;

        public T Value2;

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T* Values
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (T* ptr = &Value0)
                {
                    return ptr;
                }
            }
        }

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T this[int index]
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Values[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray4<T>
        where T : unmanaged
    {
        public T Value0;

        public T Value1;

        public T Value2;

        public T Value3;

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T* Values
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (T* ptr = &Value0)
                {
                    return ptr;
                }
            }
        }

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T this[int index]
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Values[index] = value;
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct FixedArray8<T>
        where T : unmanaged
    {
        public T Value0;

        public T Value1;

        public T Value2;

        public T Value3;

        public T Value4;

        public T Value5;

        public T Value6;

        public T Value7;

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T* Values
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                fixed (T* ptr = &Value0)
                {
                    return ptr;
                }
            }
        }

#pragma warning disable CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        public unsafe T this[int index]
#pragma warning restore CS0227 // 不安全代码只会在使用 /unsafe 编译的情况下出现
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => Values[index];
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set => Values[index] = value;
        }
    }
}