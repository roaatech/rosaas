namespace Roaa.Rosas.Common.Models
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
}
