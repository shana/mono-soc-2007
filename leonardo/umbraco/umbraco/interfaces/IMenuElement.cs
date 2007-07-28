
namespace umbraco.interfaces
{
    public interface IMenuElement
    {
        string ElementName { get;}
        string ElementIdPreFix { get;}
        string ElementClass { get;}
        int ExtraMenuWidth { get;}
    }
}
