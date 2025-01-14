using AutoMapper;
using L2X.Core.Utilities;
using L2X.Core.Validations;
using L2X.Data.Extensions;
using L2X.Data.Repositories;
using L2X.Data.Results;
using L2X.Data.Services;
using L2X.Exchange.Data.Entities;
using Microsoft.Extensions.Logging;

namespace L2X.Exchange.Data.Services;

public class MemberService(
        ILoggerFactory loggerFactory,
        IMapper mapper,
        IRepository<Member> repository,
        IValidator<Member> validator
    )
    : DataManagementService<Member, Guid>(loggerFactory, mapper, repository), IMemberService
{
    private readonly IValidator<Member> _validator = validator;

    public override async Task<ValidResult<Member>> Validate(IRequest? request)
    {
        var valid = await base.Validate(request);
        if (!valid.Success) return valid;

        return valid.IsValid(_validator);
    }

    protected override void BuildQuery(FilterRequest? filter)
    {
        base.BuildQuery(filter);
    }

    public async Task<IResult<MemberResponse>> GetByIdOrName(string value)
    {
        var result = Result.New<MemberResponse>();

        try
        {
            var uid = new Uuid(value);

            Member? member = null;
            if (!uid.IsEmpty)
                member = await Repository.GetById<Member, Guid>(uid);

            if (member == null && !Util.IsEmpty(value))
            {
                value = value.Trim();
                member = await Repository.Query(o => o.Name == value || o.Email == value).GetFirst();
            }

            if (member == null) return result.BadRequest("Request value must be OrderId or OrderNo");

            var data = Mapper.Map<Member, MemberResponse>(member);
            if (data == null) return result.BadRequest("Result data can not be mapped as usual");
            return result.Ok(data);
        }
        catch (Exception ex)
        {
            return result.Error(ex);
        }
    }
}