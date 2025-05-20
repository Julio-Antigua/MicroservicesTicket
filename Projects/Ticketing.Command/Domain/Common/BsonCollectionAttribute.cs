namespace Ticketing.Command.Domain.Common
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false)]
    public class BsonCollectionAttribute : Attribute
    {
        public BsonCollectionAttribute(string collecttionName)
        {
                CollectionName = collecttionName;
        }

        public string CollectionName { get; set; }
    }
}
