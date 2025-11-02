using SmartNotificationLibrary.Enums;
using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinCommon.Util;

namespace SmartNotificationManger.Entities
{
    public class KCustomPriorityApp : ObservableObject
    {
        private string _id = string.Empty;
        private string _PackageFamilyName = string.Empty;
        private Priority _priority = Priority.None;

        [PrimaryKey]
        public string Id
        {
            get => _id;
            set { _id = value; OnPropertyChanged(); }
        }

        [Indexed]
        public string PackageFamilyName
        {
            get => _PackageFamilyName;
            set { _PackageFamilyName = value; OnPropertyChanged(); }
        }


        [Indexed]
        public Priority Priority
        {
            get => _priority;
            set { _priority = value; OnPropertyChanged(); }
        }

      
     
        /// <summary>
        /// Updates this entity with new data
        /// </summary>
        public void Update(KCustomPriorityApp newData)
        {
            if (newData == null) return;

            Priority = newData.Priority;
        }

        /// <summary>
        /// Creates a deep clone of this entity
        /// </summary>
        public KCustomPriorityApp DeepClone()
        {
            return new KCustomPriorityApp
            {
                Id = Id,
                PackageFamilyName = PackageFamilyName,
                Priority = Priority,
            };
        }
    }

}
