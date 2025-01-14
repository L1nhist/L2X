using L2X.Data.Results;
using L2X.Data.Services;
using L2X.Exchange.Data.Entities;

namespace L2X.Exchange.Data.Services;

public interface IMemberService : IDataManagementService<Member, Guid>
{
    Task<IResult<MemberResponse>> GetByIdOrName(string value);
}