using System.Dynamic;

namespace thehomebrewapi.Helpers
{
    public interface IDataManipulation
    {
        ExpandoObject ShapeData<TSource>(TSource source, string fields);
    }
}
