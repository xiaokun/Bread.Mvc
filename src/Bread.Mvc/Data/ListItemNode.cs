namespace Bread.Mvc;


public record class ListItemNode<T>(string Title, T Value)
{
    public override string ToString()
    {
        return Title;
    }
}


public interface IEnumDescriptioner<T> where T : Enum
{
   string GetDescription(T value);
}