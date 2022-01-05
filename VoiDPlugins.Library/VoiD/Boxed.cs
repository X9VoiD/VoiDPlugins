namespace VoiDPlugins.Library
{
    public class Boxed<T> where T : unmanaged
    {
        public Boxed(T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}