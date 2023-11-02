using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public class ProcessNoteModel
    {
        public UserType OwnerType { get; set; }

        public string Text { get; set; } = string.Empty;
    }
}
