namespace ConferencePlanner.GraphQL.Types
{
    using Bogus;

    public class MeetingType : ObjectType<Meeting>
    {
        protected override void Configure(IObjectTypeDescriptor<Meeting> descriptor)
        {
            //descriptor.Field(f => f.DomainEvents).Ignore();
            descriptor.Field(f => f.Participiants).UseFiltering();
            descriptor.Field("randomFiled").Resolve(ctx => new Faker().Random.Word())
                .Type<NonNullType<StringType>>()
                .Name("randomField");
        }
    }
}
