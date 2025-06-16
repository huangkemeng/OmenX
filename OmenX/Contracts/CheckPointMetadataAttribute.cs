using System;

namespace OmenX.Contracts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CheckPointMetadataAttribute : Attribute
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public CheckPointMetadataAttribute()
        {
            
        }
        public CheckPointMetadataAttribute(string title, string description = null)
        {
            Title = title;
            Description = description;
        }
    }
}