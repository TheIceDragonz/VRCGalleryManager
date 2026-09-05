using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VRCGalleryManager.Core.Api.Models
{
    public class InventoryMetadata
    {
        [JsonPropertyName("fileId")]
        public string? FileId { get; set; }

        [JsonPropertyName("imageUrl")]
        public string? ImageUrl { get; set; }

        [JsonPropertyName("maskTag")]
        public string? MaskTag { get; set; }

        [JsonPropertyName("animated")]
        public bool Animated { get; set; }

        [JsonPropertyName("animationStyle")]
        public string? AnimationStyle { get; set; }

        [JsonPropertyName("frames")]
        public int Frames { get; set; }

        [JsonPropertyName("framesOverTime")]
        public int FramesOverTime { get; set; }

        [JsonPropertyName("loopStyle")]
        public string? LoopStyle { get; set; }
    }

    public class InventoryItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = "";

        [JsonPropertyName("name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("description")]
        public string Description { get; set; } = "";

        [JsonPropertyName("imageUrl")]
        public string ImageUrl { get; set; } = "";

        [JsonPropertyName("holderId")]
        public string HolderId { get; set; } = "";

        [JsonPropertyName("itemType")]
        public string ItemType { get; set; } = "";

        [JsonPropertyName("itemTypeLabel")]
        public string ItemTypeLabel { get; set; } = "";

        [JsonPropertyName("isArchived")]
        public bool IsArchived { get; set; }

        [JsonPropertyName("isSeen")]
        public bool IsSeen { get; set; }

        [JsonPropertyName("collections")]
        public List<string> Collections { get; set; } = new();

        [JsonPropertyName("flags")]
        public List<string> Flags { get; set; } = new();

        [JsonPropertyName("tags")]
        public List<string> Tags { get; set; } = new();

        [JsonPropertyName("metadata")]
        public InventoryMetadata? Metadata { get; set; }

        [JsonPropertyName("templateId")]
        public string? TemplateId { get; set; }

        [JsonPropertyName("templateCreatedAt")]
        public DateTime TemplateCreatedAt { get; set; }

        [JsonPropertyName("templateUpdatedAt")]
        public DateTime TemplateUpdatedAt { get; set; }

        [JsonPropertyName("createdAt")]
        public DateTime CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public DateTime UpdatedAt { get; set; }

        [JsonPropertyName("expiryDate")]
        public DateTime? ExpiryDate { get; set; }
    }

    public class InventoryResponse
    {
        [JsonPropertyName("data")]
        public List<InventoryItem> Data { get; set; } = new();

        [JsonPropertyName("totalCount")]
        public int TotalCount { get; set; }
    }
}
