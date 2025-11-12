using SmartNotificationLibrary.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartNotificationManger.Entities.Entities
{
    public class ExcludePackageMapper
    {
        [PrimaryKey]
        public string PackageFamilyName { get; set; } = string.Empty;

        [Indexed]
        public string UserId { get; set; } = string.Empty;

    }
}
