using WinCommon.Util;

namespace SmartNotificationManger.Entities
{
    public class KSpace : ObservableObject
    {
        public string SpaceId { get; set; }

        private string _SpaceName;

        public string SpaceName
        {
            get { return _SpaceName; }
            set => SetIfDifferent(ref _SpaceName, value);
        }

        private string _SpaceDescription;

        public string SpaceDescription
        {
            get { return _SpaceDescription; }
            set => SetIfDifferent(ref _SpaceDescription, value);
        }

        public bool IsDefaultWorkSpace { get; set; } = true;

        public void Update(KSpace other)
        {
            if (other == null) return;

            SpaceId = other.SpaceId;
            SpaceName = other.SpaceName;
            SpaceDescription = other.SpaceDescription;
            IsDefaultWorkSpace = other.IsDefaultWorkSpace;
        }

        public KSpace DeepClone()
        {
            return new KSpace
            {
                SpaceId = this.SpaceId,
                SpaceName = this.SpaceName,
                SpaceDescription = this.SpaceDescription,
                IsDefaultWorkSpace = this.IsDefaultWorkSpace
            };
        }
    }
}
