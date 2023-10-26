namespace Arch.SystemGroups
{
    /// <summary>
    /// Publicly available extensions for the ArchSystemsWorldBuilder
    /// </summary>
    public static class ArchSystemsWorldBuilderExtensions
    {
        /// <summary>
        /// Inject a custom group into the world. It allows to create a group with custom parameters.
        /// </summary>
        public static ref ArchSystemsWorldBuilder<T> InjectCustomGroup<T, TGroup>(ref this ArchSystemsWorldBuilder<T> builder, TGroup group) where TGroup : CustomGroupBase<float>
        {
            builder.AddCustomGroup(group);
            return ref builder;
        }
    }
}