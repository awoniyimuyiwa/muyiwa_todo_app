using System;

namespace Domain.Generic
{
    public abstract class BaseDto
    {
        /// <summary>
        /// Slug represents the string representation of a unique identifier that can be publicly exposed on an entity.
        /// It could be a pseudo-randomly generated string, the hash of the primary key (integer or guid converted to string) but not the primary key itself
        /// so as not to expose details about number of items in storage publicly.
        /// </summary>
        public string Slug { get; set; }
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }
    }
}
