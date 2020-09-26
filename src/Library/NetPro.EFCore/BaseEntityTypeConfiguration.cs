using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace NetPro.EFCore
{
    public abstract class BaseEntityTypeConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : class
    {
        /// <summary>
        /// Developers can override this method in custom partial classes in order to add some custom configuration code
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        protected virtual void PostConfigure(EntityTypeBuilder<TEntity> builder)
        {
        }

        /// <summary>
        /// Configures the entity
        /// </summary>
        /// <param name="builder">The builder to be used to configure the entity</param>
        public abstract void Configure(EntityTypeBuilder<TEntity> builder);

    }
}
