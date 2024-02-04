namespace Roaa.Rosas.Common.Models
{
    public class LookupItemDto<TId> : LookupItemDto<TId, string>
    {
        #region constructors
        public LookupItemDto()
        {

        }

        public LookupItemDto(TId id, string systemName) : base(id, systemName)
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
            SystemName = name;
        }
        #endregion

        public TId Id { get; set; }
        public TName? SystemName { get; set; }

    }

    public class CustomLookupItemDto<TId>
    {
        #region constructors
        public CustomLookupItemDto()
        {

        }

        public CustomLookupItemDto(TId id, string systemName, string displayName)
        {
            Id = id;
            SystemName = systemName;
            DisplayName = displayName;
        }
        #endregion

        public TId Id { get; set; }
        public string SystemName { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;

    }

}
