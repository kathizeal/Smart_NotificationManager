namespace SmartNotificationManger.Entities
{
    public class SpaceInfo
    {
        public bool HasSpaces { get; set; }
        public List<string> Spaces { get; set; } = new();
        public string SpacesText { get; set; } = "";
        public string SpaceIcon { get; set; } = "";
    }

}
