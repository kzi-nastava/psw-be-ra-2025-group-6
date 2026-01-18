
namespace Explorer.Blog.Core.Domain
{
    public class BlogContentItem
    {
        public int Order { get; private set; }
        public ContentType Type { get; private set; }
        public string Content { get; private set; }

        private BlogContentItem() { }

        public BlogContentItem(int order, ContentType type, string content)
        {
            if(string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.");

            Order = order;
            Type = type;
            Content = content;
        }
    }

    public enum ContentType
    {
        Text,
        Image
    }
}
