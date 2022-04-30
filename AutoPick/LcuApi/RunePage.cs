namespace AutoPick.Runes.Api
{
    using System.Text.Json.Serialization;

    public class RunePage
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("autoModifiedSelections")]
        public int[] AutoModifiedSelections { get; set; }

        [JsonPropertyName("current")]
        public bool IsCurrent { get; set; }

        [JsonPropertyName("isActive")]
        public bool IsActive { get; set; }

        [JsonPropertyName("isDeletable")]
        public bool IsDeletable { get; set; }

        [JsonPropertyName("isEditable")]
        public bool IsEditable { get; set; }

        [JsonPropertyName("isValid")]
        public bool IsValid { get; set; }

        [JsonPropertyName("lastModified")]
        public long LastModified { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("order")]
        public int Order { get; set; }

        [JsonPropertyName("primaryStyleId")]
        public int PrimaryStyleId { get; set; }

        [JsonPropertyName("subStyleId")]
        public int SecondaryStyleId { get; set; }

        [JsonPropertyName("selectedPerkIds")]
        public int[] SelectedPerkIds { get; set; }
    }
}