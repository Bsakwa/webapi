using HotChocolate.Types;
using UserApi.Models;

namespace UserApi.GraphQL.Types
{
    public class UserType : ObjectType<User>
    {
        protected override void Configure(IObjectTypeDescriptor<User> descriptor)
        {
            descriptor.Field(f => f.UserId).Type<NonNullType<IntType>>();
            descriptor.Field(f => f.Username).Type<NonNullType<StringType>>();
            descriptor.Field(f => f.Email).Type<NonNullType<StringType>>();
            descriptor.Field(f => f.PasswordHash).Ignore(); // Don't expose password hash
            descriptor.Field(f => f.CreatedAt).Type<NonNullType<DateTimeType>>();
            descriptor.Field(f => f.UpdatedAt).Type<NonNullType<DateTimeType>>();
        }
    }
}
