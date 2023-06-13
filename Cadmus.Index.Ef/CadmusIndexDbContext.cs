using Microsoft.EntityFrameworkCore;

namespace Cadmus.Index.Ef;

/// <summary>
/// Cadmus index database context.
/// </summary>
/// <seealso cref="DbContext" />
public class CadmusIndexDbContext : DbContext
{
    public DbSet<EfIndexItem> Items { get; set; }
    public DbSet<EfIndexPin> Pins { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CadmusGraphDbContext"/> class.
    /// </summary>
    public CadmusIndexDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorpusDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public CadmusIndexDbContext(DbContextOptions<CadmusIndexDbContext> options)
        : base(options)
    {
        // https://stackoverflow.com/questions/42616408/entity-framework-core-multiple-connection-strings-on-same-dbcontext
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorpusDbContext"/> class.
    /// This can be used to derive your own context which should use a typed
    /// <see cref="DbContextOptions{TContext}"/> in its constructor.
    /// </summary>
    /// <param name="options">The options for this context.</param>
    protected CadmusIndexDbContext(DbContextOptions options) : base(options)
    {
        // https://stackoverflow.com/questions/41829229/how-do-i-implement-dbcontext-inheritance-for-multiple-databases-in-ef7-net-co
    }

    /// <summary>
    /// Override this method to further configure the model that was discovered
    /// by convention from the entity types
    /// exposed in <see cref="T:Microsoft.EntityFrameworkCore.DbSet`1" />
    /// properties on your derived context. The resulting model may be cached
    /// and re-used for subsequent instances of your derived context.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model
    /// for this context.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // item
        modelBuilder.Entity<EfIndexItem>().ToTable("item");
        modelBuilder.Entity<EfIndexItem>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.Title).HasColumnName("title").HasMaxLength(500)
                .IsRequired();
            x.Property(x => x.Description).HasColumnName("description")
                .HasMaxLength(1000).IsRequired();
            x.Property(x => x.FacetId).HasColumnName("facet_id")
                .HasMaxLength(100).IsRequired();
            x.Property(x => x.GroupId).HasColumnName("group_id").HasMaxLength(100);
            x.Property(x => x.SortKey).HasColumnName("sort_key")
                .HasMaxLength(1000).IsRequired();
            x.Property(x => x.Flags).HasColumnName("flags").IsRequired();
            x.Property(x => x.TimeCreated).HasColumnName("time_created").IsRequired();
            x.Property(x => x.TimeModified).HasColumnName("time_modified").IsRequired();
            x.Property(x => x.CreatorId).HasColumnName("creator_id").HasMaxLength(100);
            x.Property(x => x.UserId).HasColumnName("user_id").HasMaxLength(100);
        });

        // pin
        modelBuilder.Entity<EfIndexPin>().ToTable("pin");
        modelBuilder.Entity<EfIndexPin>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.ItemId).HasColumnName("item_id").IsRequired()
                .HasMaxLength(36).IsFixedLength();
            x.Property(x => x.PartId).HasColumnName("part_id").IsRequired()
                .HasMaxLength(36).IsFixedLength();
            x.Property(x => x.PartTypeId).HasColumnName("part_type_id")
                .HasMaxLength(100).IsRequired();
            x.Property(x => x.PartRoleId).HasColumnName("part_role_id")
                .HasMaxLength(100);
            x.Property(x => x.Name).HasColumnName("name")
                .HasMaxLength(100).IsRequired();
            x.Property(x => x.Value).HasColumnName("value")
                .HasMaxLength(500).IsRequired();
            x.Property(x => x.TimeIndexed).HasColumnName("time_indexed")
                .IsRequired();
        });

        modelBuilder.Entity<EfIndexItem>()
            .HasMany(x => x.Pins)
            .WithOne(x => x.Item)
            .HasForeignKey(x => x.ItemId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}