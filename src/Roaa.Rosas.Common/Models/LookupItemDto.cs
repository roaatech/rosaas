﻿namespace Roaa.Rosas.Common.Models
{
    public class LookupItemDto<TId> : LookupItemDto<TId, string>
    {
        #region constructors
        public LookupItemDto()
        {

        }

        public LookupItemDto(TId id, string name) : base(id, name)
        {
        }
        #endregion


    }

    public class LookupItemDto<TId, TName>
    {
        #region constructors
        public LookupItemDto()
        {

        }

        public LookupItemDto(TId id, TName name)
        {
            Id = id;
            Name = name;
        }
        #endregion

        public TId Id { get; set; }
        public TName? Name { get; set; }

    }

    public class CustomLookupItemDto<TId>
    {
        #region constructors
        public CustomLookupItemDto()
        {

        }

        public CustomLookupItemDto(TId id, string name, string title)
        {
            Id = id;
            Name = name;
            Title = title;
        }
        #endregion

        public TId Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

    }
}
