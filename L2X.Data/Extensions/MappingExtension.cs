namespace L2X.Data.Extensions;

public static class MappingExtension
{
    public static IEnumerable<TDes> MapList<TSrc, TDes>(this IMapper mapper, IEnumerable<TSrc> lst)
        => mapper.Map<IEnumerable<TDes>>(lst);

    public static Pagination<TDes> MapPagine<TSrc, TDes>(this IMapper mapper, IEnumerable<TSrc> lst)
        => new(0, 0, lst.Count(), MapList<TSrc, TDes>(mapper, lst));

    public static Pagination<TDes> MapPagine<TSrc, TDes>(this IMapper mapper, Pagination<TSrc> pgs)
        => new(pgs.PageIndex, pgs.PageSize, pgs.TotalCount, MapList<TSrc, TDes>(mapper, pgs));
}