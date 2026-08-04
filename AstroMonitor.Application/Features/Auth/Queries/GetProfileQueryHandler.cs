using AstroMonitor.Application.Common.Interfaces;
using Dapper;
using MediatR;

namespace AstroMonitor.Application.Features.Auth.Queries;

public class GetProfileQueryHandler : IRequestHandler<GetProfileQuery, ProfileDto>
{
    private readonly ISqlConnectionFactory _sqlConnection;

    public GetProfileQueryHandler(ISqlConnectionFactory sqlConnection)
    {
        _sqlConnection = sqlConnection;
    }
    
    public async Task<ProfileDto> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        using var connection = _sqlConnection.CreateConnection();

        var sql = """
                  SELECT "FirstName", "LastName", "Email", "LastLoginDate" AS LastLogin 
                  FROM "AspNetUsers" 
                  WHERE "Id" = @Id
                  """;

        var profile = await connection
            .QueryFirstOrDefaultAsync<ProfileDto>(sql, new {id = request.UserId });
        
        return profile;
    }
}