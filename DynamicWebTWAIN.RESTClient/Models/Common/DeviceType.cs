
namespace DynamicWebTWAIN.RestClient
{
    public enum EnumDeviceTypeMask : long
    {
        DT_UNKNOWN = 0,
        DT_TWAINSCANNER = 0x10,
        DT_WIASCANNER = 0x20,
        DT_TWAINX64SCANNER = 0x40,
        DT_ICASCANNER = 0x80,
        DT_SANESCANNER = 0x100,
        DT_ESCLSCANNER = 0x200,
        DT_WIFIDIRECTSCANNER = 0x400,
        DT_WIATWAINSCANNER = 0x800,
    };
}
