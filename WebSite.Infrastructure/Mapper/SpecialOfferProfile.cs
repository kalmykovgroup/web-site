using AutoMapper;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebSite.Domain.Contracts.Dtos.SpecialOffers;
using WebSite.Domain.Models;

namespace WebSite.Infrastructure.Mapper
{
    public class SpecialOfferProfile : Profile
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public SpecialOfferProfile()
        {
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            // Entity -> DTO
            CreateMap<SpecialOffer, SpecialOfferDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => Enum.Parse<OfferType>(src.Type)))
                .ForMember(dest => dest.DisplayConfig, opt => opt.MapFrom(src =>
                    JsonSerializer.Deserialize<OfferDisplayConfigDto>(src.DisplayConfig, _jsonOptions)))
                .ForMember(dest => dest.Timer, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Timer) ? null : JsonSerializer.Deserialize<OfferTimerDto>(src.Timer, _jsonOptions)))
                .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src =>
                    string.IsNullOrEmpty(src.Metadata) ? null : JsonSerializer.Deserialize<OfferMetadataDto>(src.Metadata, _jsonOptions)));

            // DTO -> Entity
            CreateMap<SpecialOfferDto, SpecialOffer>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.DisplayConfig, opt => opt.MapFrom(src =>
                    JsonSerializer.Serialize(src.DisplayConfig, _jsonOptions)))
                .ForMember(dest => dest.Timer, opt => opt.MapFrom(src =>
                    src.Timer == null ? null : JsonSerializer.Serialize(src.Timer, _jsonOptions)))
                .ForMember(dest => dest.Metadata, opt => opt.MapFrom(src =>
                    src.Metadata == null ? null : JsonSerializer.Serialize(src.Metadata, _jsonOptions)));
        }
    }
}
