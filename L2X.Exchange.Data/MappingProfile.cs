using AutoMapper;
using L2X.Exchange.Data.Entities;

namespace L2X.Exchange.Data;

public class MappingProfile : Profile
{
	public MappingProfile()
	{
		CreateMap<MemberRequest, Member>()
			.ForMember(m => m.RoleId, map => map.MapFrom(m => Uuid.ParseGuid(m.Role)))
			.ForMember(m => m.GroupId, map => map.MapFrom(m => Uuid.ParseGuid(m.Group)));

		CreateMap<Member, MemberResponse>();

		CreateMap<Match, MatchFillResponse>()
			.ForMember(m => m.Price, map => map.MapFrom(m => m.Price))
			.ForMember(m => m.Volume, map => map.MapFrom(m => m.Volume))
			.ForMember(m => m.CreatedAt, map => map.MapFrom(m => new Epoch(m.CreatedAt)))
			.ForMember(m => m.Market, map => map.MapFrom(m => m.Market.Name))
			.ForMember(m => m.Buyer, map => map.MapFrom(m => m.TakerType ? m.Taker.Name : m.Maker.Name))
			.ForMember(m => m.Seller, map => map.MapFrom(m => m.TakerType ? m.Maker.Name : m.Taker.Name))
			.ForMember(m => m.BuyPrice, map => map.MapFrom(m => m.TakerType ? m.TakerOrder.Price : m.MakerOrder.Price))
			.ForMember(m => m.BuyVolume, map => map.MapFrom(m => m.TakerType ? m.TakerOrder.Volume : m.MakerOrder.Volume))
			.ForMember(m => m.SellPrice, map => map.MapFrom(m => m.TakerType ? m.MakerOrder.Price : m.TakerOrder.Price))
			.ForMember(m => m.SellVolume, map => map.MapFrom(m => m.TakerType ? m.MakerOrder.Volume : m.TakerOrder.Volume));

		CreateMap<OrderRequest, Order>()
			.ForMember(o => o.OwnerId, map => map.MapFrom(o => new Uuid(o.Owner)));

		CreateMap<Order, OrderResponse>()
			.ForMember(o => o.Id, map => map.MapFrom(o => new Uuid(o.Id)))
			.ForMember(o => o.Owner, map => map.MapFrom(o => o.Owner != null ? o.Owner.Name ?? "" : ""))
			.ForMember(o => o.Market, map => map.MapFrom(o => o.Market != null ? o.Market.Name ?? "" : ""))
			.ForMember(o => o.CreatedAt, map => map.MapFrom(o => new Epoch(o.CreatedAt)))
			.ForMember(o => o.ModifiedAt, map => map.MapFrom(o => new Epoch(o.ModifiedAt ?? 0)))
			.ForMember(o => o.ExpiredAt, map => map.MapFrom(o => new Epoch(o.ExpiredAt ?? 0)))
			.ForMember(o => o.FinishedAt, map => map.MapFrom(o => new Epoch(o.FinishedAt ?? 0)));

		CreateMap<PreOrder, Order>()
			.ForMember(o => o.OrderNo, map => map.MapFrom(p => p.Id));

        CreateMap<Order, PreOrder>()
            .ForMember(p => p.Id, map => map.MapFrom(o => o.OrderNo)).ReverseMap();

        CreateMap<PreOrder, PreOrderResponse>()
			.ForMember(p => p.Owner, map => map.MapFrom(p => p.Owner != null ? p.Owner.Name ?? "" : ""))
			.ForMember(p => p.Market, map => map.MapFrom(p => p.Market != null ? p.Market.Name ?? "" : ""))
			.ForMember(p => p.CreatedAt, map => map.MapFrom(p => new Epoch(p.CreatedAt)))
			.ForMember(p => p.ExpiredAt, map => map.MapFrom(p => new Epoch(p.ExpiredAt ?? 0)));

	}
}