using Microsoft.EntityFrameworkCore;

namespace Cadmus.Graph.Ef;

/// <summary>
/// Cadmus graph database context.
/// </summary>
/// <seealso cref="DbContext" />
public class CadmusGraphDbContext : DbContext
{
    public DbSet<EfUriEntry> UriEntries { get; set; }
    public DbSet<EfNode> Nodes { get; set; }
    public DbSet<EfProperty> Properties { get; set; }
    public DbSet<EfNodeClass> NodeClasses { get; set; }
    public DbSet<EfNamespaceEntry> NamespaceEntries { get; set; }
    public DbSet<EfUidEntry> UidEntries { get; set; }
    public DbSet<EfMapping> Mappings { get; set; }
    public DbSet<EfMappingLink> MappingLinks { get; set; }
    public DbSet<EfMappingMetaOutput> MappingMetaOutputs { get; set; }
    public DbSet<EfMappingNodeOutput> MappingNodeOutputs { get; set; }
    public DbSet<EfMappingTripleOutput> MappingTripleOutputs { get; set; }
    public DbSet<EfTriple> Triples { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CadmusGraphDbContext"/> class.
    /// </summary>
    public CadmusGraphDbContext()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CorpusDbContext"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public CadmusGraphDbContext(DbContextOptions<CadmusGraphDbContext> options)
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
    protected CadmusGraphDbContext(DbContextOptions options) : base(options)
    {
        // https://stackoverflow.com/questions/41829229/how-do-i-implement-dbcontext-inheritance-for-multiple-databases-in-ef7-net-co
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // uri_lookup
        modelBuilder.Entity<EfUriEntry>().ToTable("uri_lookup");
        modelBuilder.Entity<EfUriEntry>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.Uri).HasColumnName("uri").IsRequired().HasMaxLength(500);
        });
        // one-to-one relationship where the dependent entity's primary key
        // is also its foreign key to the principal entity (shared primary key
        // association)
        modelBuilder.Entity<EfUriEntry>()
             .HasOne(p => p.Node)
             .WithOne(d => d.UriEntry)
             .HasForeignKey<EfNode>(d => d.Id);

        // node
        modelBuilder.Entity<EfNode>().ToTable("node");
        modelBuilder.Entity<EfNode>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.IsClass).IsRequired().HasColumnName("is_class");
            x.Property(x => x.Tag).HasColumnName("tag").HasMaxLength(50);
            x.Property(x => x.Label).HasColumnName("label").HasMaxLength(500);
            x.Property(x => x.SourceType).IsRequired().HasColumnName("source_type");
            x.Property(x => x.Sid).HasColumnName("sid").HasMaxLength(500);
        });
        modelBuilder.Entity<EfNode>()
            .HasOne(n => n.Property)
            .WithOne(p => p.Node)
            .HasForeignKey<EfProperty>(p => p.Id);

        // property
        modelBuilder.Entity<EfProperty>().ToTable("property");
        modelBuilder.Entity<EfProperty>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.DataType).HasColumnName("data_type").HasMaxLength(100);
            x.Property(x => x.LitEditor).HasColumnName("lit_editor").HasMaxLength(100);
            x.Property(x => x.Description).HasColumnName("description").HasMaxLength(5000);
        });

        // node_class
        modelBuilder.Entity<EfNodeClass>().ToTable("node_class");
        modelBuilder.Entity<EfNodeClass>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.NodeId).IsRequired().HasColumnName("node_id");
            x.Property(x => x.ClassId).IsRequired().HasColumnName("class_id");
            x.Property(x => x.Level).IsRequired().HasColumnName("level");
        });
        modelBuilder.Entity<EfNodeClass>()
            .HasOne(n => n.Node)
            .WithMany(n => n.Classes)
            .HasForeignKey(n => n.NodeId);

        // namespace_lookup
        modelBuilder.Entity<EfNamespaceEntry>().ToTable("namespace_lookup");
        modelBuilder.Entity<EfNamespaceEntry>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").IsRequired().HasMaxLength(50);
            x.Property(x => x.Uri).HasColumnName("uri").IsRequired().HasMaxLength(500);
        });

        // uid_lookup
        modelBuilder.Entity<EfUidEntry>().ToTable("uid_lookup");
        modelBuilder.Entity<EfUidEntry>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.Sid).HasColumnName("sid")
                .IsRequired().HasMaxLength(500);
            x.Property(x => x.Unsuffixed).HasColumnName("unsuffixed")
                .IsRequired().HasMaxLength(500);
            x.Property(x => x.HasSuffix).IsRequired().HasColumnName("has_suffix");
        });

        // mapping_out_meta
        modelBuilder.Entity<EfMappingMetaOutput>().ToTable("mapping_out_meta");
        modelBuilder.Entity<EfMappingMetaOutput>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.MappingId).IsRequired().HasColumnName("mapping_id");
            x.Property(x => x.Ordinal).IsRequired().HasColumnName("ordinal");
            x.Property(x => x.Name).IsRequired().HasColumnName("name").HasMaxLength(100);
            x.Property(x => x.Value).IsRequired().HasColumnName("value").HasMaxLength(10000);
        });

        // mapping_out_node
        modelBuilder.Entity<EfMappingNodeOutput>().ToTable("mapping_out_node");
        modelBuilder.Entity<EfMappingNodeOutput>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.MappingId).IsRequired().HasColumnName("mapping_id");
            x.Property(x => x.Ordinal).IsRequired().HasColumnName("ordinal");
            x.Property(x => x.Name).IsRequired().HasColumnName("name").HasMaxLength(100);
            x.Property(x => x.Uid).IsRequired().HasColumnName("uid").HasMaxLength(1000);
            x.Property(x => x.Label).HasColumnName("label").HasMaxLength(1000);
            x.Property(x => x.Tag).HasColumnName("tag").HasMaxLength(100);
        });

        // mapping_out_triple
        modelBuilder.Entity<EfMappingTripleOutput>().ToTable("mapping_out_triple");
        modelBuilder.Entity<EfMappingTripleOutput>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.MappingId).IsRequired().HasColumnName("mapping_id");
            x.Property(x => x.Ordinal).IsRequired().HasColumnName("ordinal");
            x.Property(x => x.S).HasColumnName("s").IsRequired().HasMaxLength(1000);
            x.Property(x => x.P).HasColumnName("p").IsRequired().HasMaxLength(1000);
            x.Property(x => x.O).HasColumnName("o").HasMaxLength(1000);
            x.Property(x => x.OL).HasColumnName("ol").HasMaxLength(10000);
        });

        // mapping_link
        modelBuilder.Entity<EfMappingLink>().ToTable("mapping_link");
        modelBuilder.Entity<EfMappingLink>(x =>
        {
            x.HasKey(x => new { x.ParentId, x.ChildId });
            x.Property(x => x.ParentId).HasColumnName("parent_id");
            x.Property(x => x.ChildId).HasColumnName("child_id");

            x.HasOne(l => l.Parent)
                .WithMany(m => m.ChildLinks)
                .HasForeignKey(l => l.ParentId)
                .OnDelete(DeleteBehavior.Cascade);

            x.HasOne(l => l.Child)
                .WithMany(m => m.ParentLinks)
                .HasForeignKey(l => l.ChildId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // mapping
        modelBuilder.Entity<EfMapping>().ToTable("mapping");
        modelBuilder.Entity<EfMapping>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.Ordinal).IsRequired().HasColumnName("ordinal");
            x.Property(x => x.Name).HasColumnName("name").IsRequired().HasMaxLength(100);
            x.Property(x => x.SourceType).HasColumnName("source_type").IsRequired();
            x.Property(x => x.FacetFilter).HasColumnName("facet_filter")
                .HasMaxLength(100);
            x.Property(x => x.GroupFilter).HasColumnName("group_filter")
                .HasMaxLength(100);
            x.Property(x => x.TitleFilter).HasColumnName("title_filter")
                .HasMaxLength(100);
            x.Property(x => x.FlagsFilter).HasColumnName("flags_filter");
            x.Property(x => x.PartTypeFilter).HasColumnName("part_type_filter")
                .HasMaxLength(100);
            x.Property(x => x.PartRoleFilter).HasColumnName("part_role_filter")
                .HasMaxLength(100);
            x.Property(x => x.Description).HasColumnName("description")
                .HasMaxLength(1000);
            x.Property(x => x.Source).HasColumnName("source").IsRequired()
                .HasMaxLength(500);
            x.Property(x => x.Sid).HasColumnName("sid").HasMaxLength(500);
            x.Property(x => x.ScalarPattern).HasColumnName("scalar_pattern")
                .HasMaxLength(500);
        });
        // output metadata
        modelBuilder.Entity<EfMapping>()
            .HasMany(m => m.MetaOutputs)
            .WithOne(m => m.Mapping)
            .HasForeignKey(m => m.MappingId);
        // output nodes
        modelBuilder.Entity<EfMapping>()
            .HasMany(m => m.NodeOutputs)
            .WithOne(m => m.Mapping)
            .HasForeignKey(m => m.MappingId);
        // output triples
        modelBuilder.Entity<EfMapping>()
            .HasMany(m => m.TripleOutputs)
            .WithOne(m => m.Mapping)
            .HasForeignKey(m => m.MappingId);

        // triple
        modelBuilder.Entity<EfTriple>().ToTable("triple");
        modelBuilder.Entity<EfTriple>(x =>
        {
            x.HasKey(x => x.Id);
            x.Property(x => x.Id).HasColumnName("id").ValueGeneratedOnAdd();
            x.Property(x => x.SubjectId).HasColumnName("s_id").IsRequired();
            x.Property(x => x.PredicateId).HasColumnName("p_id").IsRequired();
            x.Property(x => x.ObjectId).HasColumnName("o_id");
            x.Property(x => x.ObjectLiteral).HasColumnName("o_lit");
            x.Property(x => x.LiteralType).HasColumnName("o_lit_type")
                .HasMaxLength(100);
            x.Property(x => x.LiteralLanguage).HasColumnName("o_lit_lang")
                .HasMaxLength(10);
            x.Property(x => x.ObjectLiteralIx).HasColumnName("o_lit_ix")
                .HasMaxLength(15000);
            x.Property(x => x.LiteralNumber).HasColumnName("o_lit_n");
            x.Property(x => x.Sid).HasColumnName("sid").HasMaxLength(500);
            x.Property(x => x.Tag).HasColumnName("tag").HasMaxLength(50);
        });
        modelBuilder.Entity<EfTriple>()
            .HasOne(m => m.Subject)
            .WithMany(n => n.SubjectTriples)
            .HasForeignKey(m => m.SubjectId);
        modelBuilder.Entity<EfTriple>()
            .HasOne(m => m.Predicate)
            .WithMany(n => n.PredicateTriples)
            .HasForeignKey(m => m.PredicateId);
        modelBuilder.Entity<EfTriple>()
            .HasOne(m => m.Object)
            .WithMany(n => n.ObjectTriples)
            .HasForeignKey(m => m.ObjectId);
    }
}