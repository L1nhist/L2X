//using AutoMapper;
//using L2X.Exchange.Data.Entities;
//using L2X.MessageQueue.Models;
//using L2X.MessageQueue.Models.Matching;

//namespace L2X.MessageQueue;

//public class MappingProfile : Profile
//{
//	public MappingProfile()
//	{
//		CreateMap<PreOrder, MOrder>()
//			.ForMember(m => m.Id, map => map.MapFrom(o => o.CreatedAt))
//			.ForMember(m => m.Owner, map => map.MapFrom(o => o.OwnerId.ToString()))
//			.ForMember(m => m.TipVolume, map => map.MapFrom(o => o.Origin))
//			.ForMember(m => m.Timestamp, map => map.MapFrom(o => o.CreatedAt))
//			.ForMember(m => m.SelfMatch, map => map.MapFrom(o => SelfMatchAction.Match))
//			.ForMember(m => m.CancelOn, map => map.MapFrom(o => o.ExpiredAt));
//	}
//}