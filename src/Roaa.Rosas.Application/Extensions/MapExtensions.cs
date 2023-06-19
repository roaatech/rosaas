using Roaa.Rosas.Domain.Entities.Management;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Application.Extensions
{
    public static class MapExtensions
    {
        public static IEnumerable<ActionResultModel> ToActionsResults(this ICollection<Process> processes)
        {
            return processes.Select(x => new ActionResultModel(x.NextStatus, x.Name));
        }
    }
}
