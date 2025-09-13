using Microsoft.AspNetCore.Identity;
using Server.Domain.Entities;

namespace Server.Services.Stores;

public interface IApplicationContextUserStore : IQueryableUserStore<User>, IUserPasswordStore<User>
{
}