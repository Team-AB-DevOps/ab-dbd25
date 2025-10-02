using api.Models;
using Riok.Mapperly.Abstractions;

namespace api.DTOs.Mappers;

[Mapper]
public partial class MediaMapper
{
    [MapperIgnoreSource(nameof(Media.WatchLists))]
    [MapperIgnoreSource(nameof(Media.Genres))]
    [MapperIgnoreSource(nameof(Media.Episodes))]
    [MapperIgnoreSource(nameof(Media.MediaPersonRoles))]
    public partial MediaDTO ToMediaDto(Media media);

    public partial List<MediaDTO> ToMediaDtos(IEnumerable<Media> medias);
}