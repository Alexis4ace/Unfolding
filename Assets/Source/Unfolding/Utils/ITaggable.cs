
namespace Unfolding.Utils
{
    /// <summary>
    /// Interface which each taggable class should extend (i.e. to use the tag in log messages).
    /// </summary>
    public interface ITaggable
    {
        public string TAG { get; }
    }
}