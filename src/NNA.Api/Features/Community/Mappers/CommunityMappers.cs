﻿using System.Globalization;
using AutoMapper;
using NNA.Domain.DTOs.Community;
using NNA.Domain.Entities;
using NNA.Domain.Mappers;

namespace NNA.Api.Features.Community.Mappers; 

public sealed class CommunityMappers : Profile {
    public CommunityMappers() {
        CreateMap<Dmo, PublishedDmoShortDto>()
            .ForMember(d => d.DmoStatusId, dcd => dcd.MapFrom(dd => dd.DmoStatus))
            .ForMember(dmo => dmo.DmoStatus,
                dmoConfig => dmoConfig.MapFrom(dmoDto => DmoStatusMapper.GetDmoStatusString(dmoDto.DmoStatus)))
            .ForMember(d => d.AuthorNickname, dcd => dcd.MapFrom(dd => dd.NnaUser.UserName))
            .ForMember(d => d.PublishDate, dcd => dcd.MapFrom(dd =>  new DateTimeOffset(dd.PublishDate!.Value, TimeSpan.Zero).ToString("g", new CultureInfo("en-US"))))
            .ForMember(d => d.PublishDateRaw, dcd => dcd.MapFrom(dd => dd.PublishDate!.Value));
    }
}