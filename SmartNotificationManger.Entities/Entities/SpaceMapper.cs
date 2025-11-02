using SQLite;

namespace SmartNotificationManger.Entities
{
    public class KSpaceMapper
    {
        [PrimaryKey]
        public string Id { get; set; }
        public string SpaceId { get; set; }
        public string PackageFamilyName { get; set; }

        public KSpaceMapper()
        {
        }

        public KSpaceMapper(string spaceId, string packageFamilyName)
        {
            SpaceId = spaceId;
            PackageFamilyName = packageFamilyName;
            Id = SpaceId + "_" + PackageFamilyName;

        }
    }
}
