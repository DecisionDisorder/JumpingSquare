using System;
using System.Runtime.InteropServices;

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct FromServerPacket
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string playerName;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public float velocityX;
    public float velocityY;
    public float velocityZ;
}

[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi, Pack = 1)]
[Serializable]
public struct ToServerPacket
{
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 20)]
    public string playerName;

    public float positionX;
    public float positionY;
    public float positionZ;

    public float rotationX;
    public float rotationY;
    public float rotationZ;

    public float velocityX;
    public float velocityY;
    public float velocityZ;
}
